using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Data.Messages;

namespace Unity1week202504.Message
{
    public class SubTitleMessagePlayer
    {
        private readonly SubTitleMessageWindow _subTitleMessageWindow;

        private CancellationTokenSource _cancellationTokenSource = new();
        private bool _isPlaying;

        public SubTitleMessagePlayer(SubTitleMessageWindow subTitleMessageWindow)
        {
            _subTitleMessageWindow = subTitleMessageWindow;
        }

        public async UniTask PlayAsync(MessageBlock messageBlock, CancellationToken cancellationToken = default)
        {
            if (!messageBlock.IsValid) return;

            // すでに再生中ならキャンセル
            if (_isPlaying)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            _cancellationTokenSource = new CancellationTokenSource();

            var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(
                _cancellationTokenSource.Token,
                cancellationToken).Token;

            _isPlaying = true;

            if (!_subTitleMessageWindow.IsShowing)
            {
                await _subTitleMessageWindow.ShowAsync(linkedCancellationToken);
            }

            await _subTitleMessageWindow.PlaySequence(messageBlock, linkedCancellationToken);
            await _subTitleMessageWindow.HideAsync(linkedCancellationToken);

            _isPlaying = false;
        }
    }
}