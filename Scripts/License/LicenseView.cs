using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202504.License
{
    public class LicenseView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _root;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private TextMeshProUGUI _licenseText;

        [SerializeField]
        private Button _closeButton;

        public Observable<Unit> OnCloseAsObservable() => _closeButton.OnClickAsObservable();

        private void Awake()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void SetLicenseText(string licenseData)
        {
            _licenseText.text = licenseData;
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            const float duration = 0.16f;

            var sequence = LSequence.Create();
            sequence.Join(LMotion.Create(_root.anchoredPosition.y - 48f, _root.anchoredPosition.y, duration)
                .WithEase(Ease.OutBack).BindToAnchoredPositionY(_root));
            sequence.Join(LMotion.Create(0f, 1f, duration).BindToAlpha(_canvasGroup));
            sequence.AddTo(cancellationToken);
            await sequence.Run();
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            await LMotion.Create(1f, 0f, 0f)
                .BindToAlpha(_canvasGroup)
                .ToUniTask(cancellationToken);
        }
    }
}