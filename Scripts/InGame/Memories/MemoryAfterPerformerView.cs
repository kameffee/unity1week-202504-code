using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202504.InGame.Memories
{
    public class MemoryAfterPerformerView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private CanvasGroup _whiteFade;

        [SerializeField]
        private CanvasGroup _mainImageCanvasGroup;

        [SerializeField]
        private Image _mainImage;

        [SerializeField]
        private CanvasGroup _waitClick;

        [SerializeField]
        private Button _waitClickButton;

        private ViewModel _viewModel;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _waitClick.gameObject.SetActive(false);
        }

        public void Apply(ViewModel viewModel)
        {
            _viewModel = viewModel;
            _mainImage.sprite = viewModel.MainSprite;
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken)
        {
            _mainImageCanvasGroup.alpha = 0f;
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            // Whiteを表示
            _whiteFade.alpha = 0;
            await LMotion.Create(0f, 1f, 0.5f)
                .BindToAlpha(_whiteFade)
                .AddTo(this)
                .ToUniTask(cancellationToken);

            // メイン画像を表示
            _mainImageCanvasGroup.alpha = 1f;

            // Whiteを非表示
            await LMotion.Create(1f, 0f, 1f)
                .BindToAlpha(_whiteFade)
                .AddTo(this)
                .ToUniTask(cancellationToken);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken)
        {
            await LMotion.Create(0f, 1f, 0.5f)
                .BindToAlpha(_whiteFade)
                .AddTo(this)
                .ToUniTask(cancellationToken);

            _mainImageCanvasGroup.alpha = 0f;

            await LMotion.Create(1f, 0f, 0.5f)
                .BindToAlpha(_canvasGroup)
                .AddTo(this)
                .ToUniTask(cancellationToken);

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _whiteFade.alpha = 0f;
        }

        public async UniTask ShowAndWaitClickAsync(CancellationToken cancellationToken)
        {
            _waitClick.gameObject.SetActive(true);

            await _waitClickButton.OnClickAsObservable().FirstAsync(cancellationToken);

            _waitClick.gameObject.SetActive(false);
        }

        public class ViewModel
        {
            public Sprite MainSprite { get; }

            public ViewModel(Sprite mainSprite)
            {
                MainSprite = mainSprite;
            }
        }
    }
}