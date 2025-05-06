using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Audio.Player;
using Unity1week202504.Extensions;
using Unity1week202504.Message;
using Unity1week202504.Scenes;
using UnityEngine;
using VContainer.Unity;

namespace Unity1week202504.Ending
{
    public class EndingPresenter : Presenter, IInitializable, IAsyncStartable
    {
        private readonly EndingView _view;
        private readonly MemorySelectPhase _memorySelectPhase;
        private readonly SceneLoader _sceneLoader;
        private readonly EndingCalculator _endingCalculator;
        private readonly MessagePlayer _messagePlayer;
        private readonly AudioPlayer _audioPlayer;
        private readonly GameResetter _gameResetter;

        public EndingPresenter(
            EndingView view,
            MemorySelectPhase memorySelectPhase,
            SceneLoader sceneLoader,
            EndingCalculator endingCalculator,
            MessagePlayer messagePlayer,
            AudioPlayer audioPlayer,
            GameResetter gameResetter)
        {
            _view = view;
            _memorySelectPhase = memorySelectPhase;
            _sceneLoader = sceneLoader;
            _endingCalculator = endingCalculator;
            _messagePlayer = messagePlayer;
            _audioPlayer = audioPlayer;
            _gameResetter = gameResetter;
        }

        public void Initialize()
        {
            _view.OnClickReturnAsObservable()
                .SubscribeAwait((_, ct) => TransitionToTitle(ct))
                .AddTo(this);
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            var selectedMemoryIds = await _memorySelectPhase.ExecuteAsync(cancellation);
            Debug.Log($"Selected memory ids: {string.Join(", ", selectedMemoryIds)}");

            var endingMaster = _endingCalculator.Calculate(selectedMemoryIds);
            Debug.Log($"EndingId: {endingMaster.Id.AsPrimitive()}");

            _view.Apply(endingMaster.EndingName);

            await _memorySelectPhase.HideAsync(cancellation);

            // BGMを流す
            _audioPlayer.PlayBgm(endingMaster.EndingBGM);

            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellation);

            // EndingCommentを流す
            await _messagePlayer.PlayAsync(endingMaster.EndingComment, MessageMode.Auto, cancellation);

            await _view.ShowAsync(cancellation);
        }

        private async UniTask TransitionToTitle(CancellationToken cancellationToken)
        {
            await _view.HideAsync(cancellationToken);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);
            
            _gameResetter.Reset();
            
            // awaitはしない
            _sceneLoader.LoadAsync("Title").Forget();
        }
    }
}