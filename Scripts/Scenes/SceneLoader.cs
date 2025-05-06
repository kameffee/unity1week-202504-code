using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Unity1week202504.Scenes
{
    public class SceneLoader : IDisposable
    {
        private readonly ISceneTransitionView _sceneTransitionView;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private const float ShowDelayTime = 0.5f;
        private const float HideDelayTime = 0.5f;

        public SceneLoader(ISceneTransitionView sceneTransitionView)
        {
            _sceneTransitionView = sceneTransitionView;
        }

        public async UniTask LoadAsync(string sceneName, CancellationToken cancellationToken = default)
        {
            var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(
                _cancellationTokenSource.Token,
                cancellationToken).Token;

            await _sceneTransitionView.ShowAsync();

            var currentScene = SceneManager.GetActiveScene();

            await UniTask.Delay(TimeSpan.FromSeconds(ShowDelayTime), cancellationToken: linkedCancellationToken);

            // 遷移先のシーンを読み込み
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive)
                .ToUniTask(cancellationToken: linkedCancellationToken);

            // アクティブ化
            var loadedScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(loadedScene);

            // 前のシーンをアンロード
            await SceneManager.UnloadSceneAsync(currentScene)
                .ToUniTask(cancellationToken: linkedCancellationToken);

            await UniTask.Delay(TimeSpan.FromSeconds(HideDelayTime), cancellationToken: linkedCancellationToken);

            await _sceneTransitionView.HideAsync();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}