using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity1week202504.InGame.Cameras;
using Unity1week202504.InGame.Memories;
using Unity1week202504.Scenes;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer.Unity;

namespace Unity1week202504.InGame
{
    public class InGameLoop : IAsyncStartable, IInitializable
    {
        private readonly SceneLoader _sceneLoader;
        private readonly TakePhotoPhase _takePhotoPhase;
        private readonly MemoryGeneratePhase _memoryGeneratePhase;
        private readonly SnapTargetActiveSwitcher _snapTargetActiveSwitcher;
        private readonly IntroSequence _introSequence;
        private readonly InGameCamera _inGameCamera;

        private int _phaseCount;
        private const int MaxPhaseCount = 3;

        public InGameLoop(
            SceneLoader sceneLoader,
            TakePhotoPhase takePhotoPhase,
            MemoryGeneratePhase memoryGeneratePhase,
            SnapTargetActiveSwitcher snapTargetActiveSwitcher,
            IntroSequence introSequence,
            InGameCamera inGameCamera)
        {
            _sceneLoader = sceneLoader;
            _takePhotoPhase = takePhotoPhase;
            _memoryGeneratePhase = memoryGeneratePhase;
            _snapTargetActiveSwitcher = snapTargetActiveSwitcher;
            _introSequence = introSequence;
            _inGameCamera = inGameCamera;
        }

        public void Initialize()
        {
            _snapTargetActiveSwitcher.SwitchActive(TimeOfDay.Morning);
            _snapTargetActiveSwitcher.SetSnappableAll(false);
            _introSequence.Initialize();
        }

        public async UniTask StartAsync(CancellationToken cancellationToken = new())
        {
            await GameStart(cancellationToken);
        }

        private async UniTask GameStart(CancellationToken cancellationToken)
        {
            // フェーズのリセット
            _phaseCount = 1;

            await LoopCore(cancellationToken);

            GameEnd();
        }

        private async UniTask LoopCore(CancellationToken cancellationToken)
        {
            await PlayIntroAsync(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                var currentTimeOfDay = (TimeOfDay)(1 << (_phaseCount - 1));
                Assert.IsTrue(Enum.IsDefined(typeof(TimeOfDay), currentTimeOfDay),
                    $"Invalid phase count: {_phaseCount}");

                _snapTargetActiveSwitcher.SwitchActive(currentTimeOfDay);
                _snapTargetActiveSwitcher.SetSnappableAll(true);

                Debug.Log($"Phase: {currentTimeOfDay.ToLocalizedString()}");

                // 撮影パート
                await _takePhotoPhase.Execute(currentTimeOfDay, cancellationToken);

                // 思い出パート
                await _memoryGeneratePhase.Execute(currentTimeOfDay, cancellationToken);

                _inGameCamera.SetDefaultPosition();

                // 次のフェーズへ
                _phaseCount++;

                // 最大を超えたら抜ける
                if (_phaseCount > MaxPhaseCount)
                {
                    Debug.Log("Max phase count reached.");
                    break;
                }
            }

            Debug.Log("Exiting loop.");
        }

        private async UniTask PlayIntroAsync(CancellationToken cancellationToken)
        {
            Debug.Log("Playing intro...");

            await _introSequence.PlayAsync(cancellationToken)
                .SuppressCancellationThrow();

            Debug.Log("Intro finished.");
        }

        private void GameEnd()
        {
            _sceneLoader.LoadAsync("Ending").Forget();
        }
    }
}