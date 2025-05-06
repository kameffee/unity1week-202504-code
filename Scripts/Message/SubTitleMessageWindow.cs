using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using TMPro;
using Unity1week202504.Audio.Player;
using Unity1week202504.Data.Messages;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;

namespace Unity1week202504.Message
{
    public class SubTitleMessageWindow : MonoBehaviour
    {
        public bool IsShowing => _isShowing;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private AudioResource _subtitleSe;

        private AudioPlayer _audioPlayer;
        private MotionHandle _handle;
        private bool _isShowing;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _text.SetText("");
        }

        [Inject]
        private void Construct(AudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            _isShowing = true;
            _text.SetText("");

            var handle = LMotion.Create(0f, 1f, 0.2f)
                .BindToAlpha(_canvasGroup)
                .AddTo(this);

            await handle.ToUniTask(cancellationToken);

            if (handle.IsActive())
            {
                handle.Complete();
                handle.Cancel();
            }
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            var handle = LMotion.Create(1f, 0f, 0.2f)
                .BindToAlpha(_canvasGroup)
                .AddTo(this);

            await handle.ToUniTask(cancellationToken: cancellationToken);

            _text.SetText("");
            _isShowing = false;
        }

        public async UniTask PlaySequence(MessageBlock messageBlock, CancellationToken cancellationToken)
        {
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

                const float interval = 0.04f;
                var duration = messageLine.Message.Length * interval;
                _handle = LMotion.String.Create512Bytes("", messageLine.Message, duration)
                    .WithEase(Ease.Linear)
                    .WithRichText()
                    .BindToText(_text);

                var ct = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cts.Token).Token;

                // テキストに合わせてSEを鳴らす
                var disposable = Observable.Interval(TimeSpan.FromSeconds(interval))
                    .Subscribe(_ => _audioPlayer.PlaySe(_subtitleSe));

                // link
                ct.Register(disposable.Dispose);

                await _handle.ToUniTask(cancellationToken: ct);
                disposable.Dispose();

                if (_handle.IsActive())
                {
                    _handle.Complete();
                }

                await UniTask.Yield();

                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: ct);
            }
        }
    }
}