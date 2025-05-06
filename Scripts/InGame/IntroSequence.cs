using System;
using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using Unity1week202504.Audio.Player;
using Unity1week202504.Data.Messages;
using Unity1week202504.Message;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Unity1week202504.InGame
{
    public class IntroSequence : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _rootCanvasGroup;

        [SerializeField]
        private CanvasGroup _blackScreen;

        [LabelText("走る音")]
        [SerializeField]
        private AudioClip _runSound;

        [LabelText("倒れる音")]
        [SerializeField]
        private AudioClip _collapseSound;

        [SerializeField]
        private EmotionType _initialEmotionType;

        [SerializeField]
        private MessageBlock _messageBlock;

        [SerializeField]
        private MessageBlock _messageBlock2;

        [SerializeField]
        private MessageBlock _messageBlock3;

        [Title("Button")]
        [SerializeField]
        private CanvasGroup _buttonCanvasGroup;

        [SerializeField]
        private Button _skipButton;

        private MessagePlayer _messagePlayer;
        private AudioPlayer _audioPlayer;

        [Inject]
        public void Construct(
            MessagePlayer messagePlayer,
            AudioPlayer audioPlayer)
        {
            _messagePlayer = messagePlayer;
            _audioPlayer = audioPlayer;
        }

        public void Initialize()
        {
            _blackScreen.alpha = 1;
        }

        public async UniTask PlayAsync(CancellationToken cancellationToken)
        {
            _rootCanvasGroup.interactable = true;
            _rootCanvasGroup.blocksRaycasts = true;

            _buttonCanvasGroup.interactable = true;
            _buttonCanvasGroup.blocksRaycasts = true;

            var ct = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, cancellationToken);
            await UniTask.WhenAny(
                PlayCore(ct.Token),
                _skipButton.OnClickAsObservable().FirstAsync(cancellationToken).AsUniTask());

            ct.Cancel();
            Hide();
        }

        private async UniTask PlayCore(CancellationToken cancellationToken)
        {
            _messagePlayer.SetEmotion(_initialEmotionType);

            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: cancellationToken);
            _audioPlayer.PlaySe(_runSound);

            await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: cancellationToken);

            _audioPlayer.PlaySe(_collapseSound);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);

            await _messagePlayer.PlayAsync(_messageBlock, MessageMode.Step, cancellationToken);

            // 黒画面をフェードアウト
            await LMotion.Create(1f, 0f, 1f)
                .BindToAlpha(_blackScreen)
                .ToUniTask(cancellationToken);

            await _messagePlayer.PlayAsync(_messageBlock2, MessageMode.Step, cancellationToken);

            await _messagePlayer.PlayAsync(_messageBlock3, MessageMode.Step, cancellationToken);

            await LMotion.Create(1f, 0f, 0.2f)
                .BindToAlpha(_rootCanvasGroup)
                .ToUniTask(cancellationToken);
        }

        private void Hide()
        {
            _rootCanvasGroup.alpha = 0;
            _rootCanvasGroup.interactable = false;
            _rootCanvasGroup.blocksRaycasts = false;

            _buttonCanvasGroup.interactable = false;
            _buttonCanvasGroup.blocksRaycasts = false;


            _audioPlayer.StopSe();
            _messagePlayer.Hide();
        }
    }
}