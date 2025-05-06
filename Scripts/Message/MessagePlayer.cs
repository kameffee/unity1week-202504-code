using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Data.Messages;

namespace Unity1week202504.Message
{
    public class MessagePlayer
    {
        private readonly MessageWindow _messageWindow;
        private CompositeDisposable _disposable = new();
        private CancellationTokenSource _cancellationTokenSource = new();

        private bool _isPlaying;

        public MessagePlayer(MessageWindow messageWindow)
        {
            _messageWindow = messageWindow;
        }

        public void SetEmotion(EmotionType emotionType)
        {
            _messageWindow.SetEmotion(emotionType);
        }

        public async UniTask PlayAsync(
            MessageBlock messageBlock,
            MessageMode messageMode,
            CancellationToken cancellationToken = default)
        {
            if (!messageBlock.IsValid) return;

            // すでに再生中ならキャンセル
            if (_isPlaying)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            _cancellationTokenSource = new();

            var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(
                _cancellationTokenSource.Token,
                cancellationToken).Token;

            _isPlaying = true;

            if (!_messageWindow.IsShowing)
            {
                await _messageWindow.ShowAsync(messageMode, linkedCancellationToken);
            }

            await _messageWindow.PlaySequence(messageBlock, messageMode, linkedCancellationToken);
            await _messageWindow.HideAsync(linkedCancellationToken);

            _isPlaying = false;
        }

        public async UniTask WaitEndAsync(CancellationToken cancellationToken)
        {
            await UniTask.WaitUntil(() => !_isPlaying, cancellationToken: cancellationToken);
        }

        public void Hide()
        {
            // 表示されていたら閉じる
            if (_messageWindow.IsShowing)
            {
                _messageWindow.HideAsync().Forget();
            }
        }
    }
}