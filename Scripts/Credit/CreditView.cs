using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202504.Credit
{
    public class CreditView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private RectTransform _windowRoot;

        [SerializeField]
        private List<CreditInstaxView> _instaxViews;

        [SerializeField]
        private Button _closeButton;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            _instaxViews.ForEach(view => view.Initialize());

            var sequence = LSequence.Create();
            sequence.Join(LMotion.Create(0f, 1f, 0.1f)
                .WithEase(Ease.Linear)
                .BindToAlpha(_canvasGroup)
                .AddTo(this)
            );

            sequence.Join(LMotion.Create(_windowRoot.anchoredPosition.y - 50f, _windowRoot.anchoredPosition.y, 0.3f)
                .WithEase(Ease.OutBack)
                .BindToAnchoredPositionY(_windowRoot)
                .AddTo(this)
            );

            await sequence.Run().ToUniTask(cancellationToken);

            ShowInstax(cancellationToken);
        }

        private void ShowInstax(CancellationToken cancellationToken = default)
        {
            var cancellation = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                destroyCancellationToken);

            foreach (var creditInstaxView in _instaxViews)
            {
                creditInstaxView.ShowAsync(cancellation.Token).Forget();
            }
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            await LMotion.Create(1f, 0f, 0.2f)
                .WithEase(Ease.Linear)
                .BindToAlpha(_canvasGroup)
                .AddTo(this)
                .ToUniTask(cancellationToken);
        }

        public Observable<Unit> OnClickCloseAsObservable() => _closeButton.OnClickAsObservable();
    }
}