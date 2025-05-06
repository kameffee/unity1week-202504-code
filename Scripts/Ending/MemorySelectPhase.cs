using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Data;
using Unity1week202504.Data.Memories;
using Unity1week202504.Extensions;
using Unity1week202504.InGame.Memories;
using Unity1week202504.Message;
using VContainer.Unity;

namespace Unity1week202504.Ending
{
    public class MemorySelectPhase : Presenter, IStartable
    {
        private readonly MemorySelectView _view;
        private readonly MemoryRepository _memoryRepository;
        private readonly MemoryMasterDataSource _memoryMasterDataSource;
        private readonly MessagePlayer _messagePlayer;
        private readonly SubTitleMessagePlayer _subTitleMessagePlayer;
        private readonly MessageBlockMasterDataSource _messageBlockMasterDataSource;

        public MemorySelectPhase(
            MemorySelectView view,
            MemoryRepository memoryRepository,
            MemoryMasterDataSource memoryMasterDataSource,
            MessagePlayer messagePlayer,
            SubTitleMessagePlayer subTitleMessagePlayer,
            MessageBlockMasterDataSource messageBlockMasterDataSource)
        {
            _view = view;
            _memoryRepository = memoryRepository;
            _memoryMasterDataSource = memoryMasterDataSource;
            _messagePlayer = messagePlayer;
            _subTitleMessagePlayer = subTitleMessagePlayer;
            _messageBlockMasterDataSource = messageBlockMasterDataSource;
        }

        public void Start()
        {
            _view.OnSelectedMemoryAsObservable()
                .SubscribeAwait(async (memoryId, ct) =>
                {
                    var masterData = _memoryMasterDataSource.Get(memoryId);
                    await _subTitleMessagePlayer.PlayAsync(masterData.ViewingMessage, ct);
                }, AwaitOperation.Switch)
                .AddTo(this);
        }

        public async UniTask<IReadOnlyCollection<MemoryId>> ExecuteAsync(CancellationToken cancellation)
        {
            var viewModels = _memoryMasterDataSource.All
                .Select(data => data.Id)
                .Select(id => _memoryMasterDataSource.Get(id))
                .Select(data => new MemoryView.ViewModel(data.Id, data.Image, _memoryRepository.IsUnlock(data.Id)))
                .ToArray();

            var viewModel = new MemorySelectView.ViewModel(viewModels);
            _view.ApplyViewModel(viewModel);

            await _view.ShowAsync(cancellation);

            var masterData = _messageBlockMasterDataSource.Get("Ending/MemorySelect/Tutorial");
            await _messagePlayer.PlayAsync(masterData.MessageBlock, MessageMode.Step, cancellation);

            await _view.OnClickSubmitAsObservable().FirstAsync(cancellation);

            return _view.SelectedMemoryIds;
        }

        public async UniTask HideAsync(CancellationToken cancellation)
        {
            await _view.HideAsync(cancellation);
        }
    }
}