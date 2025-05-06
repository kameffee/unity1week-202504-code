using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace Unity1week202504.Scenes
{
    public class SceneTransitionView : MonoBehaviour, ISceneTransitionView
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public async UniTask ShowAsync()
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            await LMotion.Create(0f, 1f, 1f)
                .WithEase(Ease.Linear)
                .BindToAlpha(_canvasGroup)
                .AddTo(this);
        }

        public async UniTask HideAsync()
        {
            await LMotion.Create(1f, 0f, 1f)
                .WithEase(Ease.Linear)
                .BindToAlpha(_canvasGroup)
                .AddTo(this);

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}