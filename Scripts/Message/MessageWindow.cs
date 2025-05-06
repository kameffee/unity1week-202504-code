using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using R3.Triggers;
using TMPro;
using Unity1week202504.Audio.Player;
using Unity1week202504.Data.Messages;
using Unity1week202504.InGame.Actor;
using Unity1week202504.InGame.Memories;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using VContainer;

namespace Unity1week202504.Message
{
    public class MessageWindow : MonoBehaviour
    {
        public bool IsShowing => _isShowing;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private GameObject _blockRaycastObject;

        [SerializeField]
        private Button _clickableScreen;

        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private AiboView _aiboView;

        [SerializeField]
        private AudioResource _messageSe;

        [SerializeField]
        private RectTransform _waitClickObject;

        private AudioPlayer _audioPlayer;
        private MotionHandle _handle;
        private bool _isShowing;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            ActiveBlockRaycast(false);
            _text.SetText("");
            _waitClickObject.gameObject.SetActive(false);
        }

        [Inject]
        private void Construct(AudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
        }

        public async UniTask ShowAsync(MessageMode messageMode, CancellationToken cancellationToken = default)
        {
            _isShowing = true;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            ActiveBlockRaycast(IsBlockRaycast(messageMode));
            _text.SetText("");

            var handle = LMotion.Create(0f, 1f, 0.1f)
                .BindToAlpha(_canvasGroup)
                .AddTo(this);

            await handle.ToUniTask(cancellationToken).SuppressCancellationThrow();

            if (handle.IsActive())
            {
                handle.Complete();
                handle.Cancel();
            }
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            // 閉じる前に表情をデフォルトに戻す
            _aiboView.SetDefaultEmotion();

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            var handle = LMotion.Create(1f, 0f, 0.1f)
                .BindToAlpha(_canvasGroup)
                .AddTo(this);

            await handle.ToUniTask(cancellationToken).SuppressCancellationThrow();

            if (handle.IsActive())
            {
                handle.Complete();
                handle.Cancel();
            }

            _text.SetText("");
            _isShowing = false;
        }

        private static bool IsBlockRaycast(MessageMode messageMode)
        {
            return messageMode is MessageMode.Step;
        }

        private void ActiveBlockRaycast(bool isActive)
        {
            _blockRaycastObject.SetActive(isActive);
        }

        public async UniTask PlaySequence(
            MessageBlock messageBlock,
            MessageMode messageMode,
            CancellationToken cancellationToken)
        {
            ActiveBlockRaycast(IsBlockRaycast(messageMode));
            HideWaitIcon();

            if (_handle.IsActive())
            {
                _handle.Complete();
                _handle.Cancel();
            }

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            foreach (var messageLine in messageBlock.Lines)
            {
                _text.SetText("");
                _aiboView.ChangeEmotion(messageLine.EmotionType);

                const float interval = 0.04f;
                var duration = messageLine.Message.Length * interval;
                _handle = LMotion.String.Create128Bytes("", messageLine.Message, duration)
                    .WithEase(Ease.Linear)
                    .WithRichText()
                    .BindToText(_text)
                    .AddTo(this);

                var ct = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cts.Token).Token;

                // テキストに合わせてSEを鳴らす
                var disposable = Observable.Interval(TimeSpan.FromSeconds(interval))
                    .Subscribe(_ => _audioPlayer.PlaySe(_messageSe));

                // link
                ct.Register(disposable.Dispose);

                if (messageMode == MessageMode.Step)
                {
                    await UniTask.WhenAny(
                        _handle.ToUniTask(cancellationToken: ct),
                        UserInput.WaitForAny(ct)
                    );
                }
                else
                {
                    await _handle.ToUniTask(cancellationToken: ct);
                }

                disposable.Dispose();

                if (_handle.IsActive())
                {
                    _handle.Complete();
                }

                await UniTask.Yield();

                if (messageMode is MessageMode.Step)
                {
                    // クリック待ち表示
                    ShowWaitIcon();

                    await _clickableScreen.OnPointerDownAsObservable()
                        .FirstAsync(cancellationToken: ct)
                        .AddTo(this);

                    HideWaitIcon();
                }
                else
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: ct);
                }
            }
        }

        private void ShowWaitIcon()
        {
            // クリック待ち表示
            _waitClickObject.gameObject.SetActive(true);
            // 最後の文字の後ろにセット
            // 最後の文字のインデックスを取得
            int lastCharacterIndex = _text.textInfo.characterCount - 1;

            // 最後の文字の情報を取得
            var charInfo = _text.textInfo.characterInfo[lastCharacterIndex];
            _waitClickObject.anchoredPosition = charInfo.bottomRight;
        }

        private void HideWaitIcon()
        {
            _waitClickObject.gameObject.SetActive(false);
        }

        public void SetEmotion(EmotionType emotionType) => _aiboView.ChangeEmotion(emotionType);
    }
}