using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity1week202504.Data;
using Unity1week202504.Data.Memories;
using Unity1week202504.Message;
using UnityEngine;

namespace Unity1week202504.InGame.Memories
{
    public class MemoryAfterPerformer
    {
        private readonly MemoryAfterPerformerView _view;
        private readonly MessagePlayer _messagePlayer;
        private readonly MemoryConditionMasterDataSource _memoryConditionMasterDataSource;
        private readonly MemoryMasterDataSource _memoryMasterDataSource;

        public MemoryAfterPerformer(
            MemoryAfterPerformerView view,
            MessagePlayer messagePlayer,
            MemoryConditionMasterDataSource memoryConditionMasterDataSource,
            MemoryMasterDataSource memoryMasterDataSource)
        {
            _view = view;
            _messagePlayer = messagePlayer;
            _memoryConditionMasterDataSource = memoryConditionMasterDataSource;
            _memoryMasterDataSource = memoryMasterDataSource;
        }

        public async UniTask PlayAsync(MemoryConditionId memoryConditionId, CancellationToken cancellationToken)
        {
            Debug.Log("MemoryPerformer.PlayAsync");
            var conditionMasterData = _memoryConditionMasterDataSource.Get(memoryConditionId);
            var memoryMasterData = _memoryMasterDataSource.Get(conditionMasterData.OutputMemoryId);

            var viewModel = new MemoryAfterPerformerView.ViewModel(memoryMasterData.Image);
            _view.Apply(viewModel);
            await _view.ShowAsync(cancellationToken);

            await UniTask.WhenAny(
                UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken),
                UserInput.WaitForAny(cancellationToken)
            );

            // コメントメッセージを再生
            await _messagePlayer.PlayAsync(conditionMasterData.GeneratedComment, MessageMode.Step, cancellationToken);

            // クリック待ち
            await _view.ShowAndWaitClickAsync(cancellationToken);

            // 演出終了
            await _view.HideAsync(cancellationToken);
        }
    }
}