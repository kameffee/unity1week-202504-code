using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202504.Ending
{
    public class EndingView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _uiCanvasGroup;
        
        [SerializeField]
        private TextMeshProUGUI _endingNameText;

        [SerializeField]
        private Button _returnButton;

        public Observable<Unit> OnClickReturnAsObservable() => _returnButton.OnClickAsObservable();

        private void Awake()
        {
            _uiCanvasGroup.alpha = 0;
            _uiCanvasGroup.interactable = false;
            _uiCanvasGroup.blocksRaycasts = false;
        }

        public void Apply(string endingName)
        {
            _endingNameText.SetText(endingName);
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken)
        {
            _uiCanvasGroup.interactable = true;
            _uiCanvasGroup.blocksRaycasts = true;

            const float duration = 0.5f;

            var sequence = LSequence.Create();
            sequence.Join(LMotion.Create(0f, 1f, duration).BindToAlpha(_uiCanvasGroup));
            sequence.AddTo(cancellationToken);
            await sequence.Run();
        }

        public async UniTask HideAsync(CancellationToken cancellationToken)
        {
            _uiCanvasGroup.interactable = false;
            _uiCanvasGroup.blocksRaycasts = false;

            const float duration = 0.5f;

            var sequence = LSequence.Create();
            sequence.Join(LMotion.Create(1f, 0f, duration).BindToAlpha(_uiCanvasGroup));
            sequence.AddTo(cancellationToken);
            await sequence.Run();
        }
    }
}