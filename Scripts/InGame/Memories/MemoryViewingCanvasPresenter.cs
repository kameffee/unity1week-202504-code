using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Data.Memories;
using Unity1week202504.Extensions;
using Unity1week202504.Message;
using VContainer.Unity;

namespace Unity1week202504.InGame.Memories
{
    public class MemoryViewingCanvasPresenter : Presenter, IInitializable
    {
        private readonly MemoryViewingCanvas _view;
        private readonly MemoryMasterDataSource _memoryMasterDataSource;
        private readonly MemoryRepository _memoryRepository;
        private readonly SubTitleMessagePlayer _subTitleMessagePlayer;
        private readonly Subject<Unit> _onClose = new();

        public MemoryViewingCanvasPresenter(
            MemoryViewingCanvas view,
            MemoryMasterDataSource memoryMasterDataSource,
            MemoryRepository memoryRepository,
            SubTitleMessagePlayer subTitleMessagePlayer)
        {
            _view = view;
            _memoryMasterDataSource = memoryMasterDataSource;
            _memoryRepository = memoryRepository;
            _subTitleMessagePlayer = subTitleMessagePlayer;
        }

        public void Initialize()
        {
            _view.OnClickCloseAsObservable()
                .SubscribeAwait(async (_, ct) =>
                {
                    await HideAsync(ct);
                }, AwaitOperation.Drop)
                .AddTo(this);

            _view.OnSelectedMemoryAsObservable()
                .SubscribeAwait(async (memoryId, ct) =>
                {
                    var masterData = _memoryMasterDataSource.Get(memoryId);
                    await _subTitleMessagePlayer.PlayAsync(masterData.ViewingMessage, ct);
                }, AwaitOperation.Switch)
                .AddTo(this);
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken)
        {
            _view.ApplyViewModel(CreateViewModel());
            await _view.ShowAsync(cancellationToken);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken)
        {
            await _view.HideAsync(cancellationToken);
            _onClose.OnNext(Unit.Default);
        }

        public Observable<Unit> OnCloseAsObservable() => _onClose;

        private MemoryViewingCanvas.ViewModel CreateViewModel()
        {
            var viewModels = _memoryMasterDataSource.All
                .Select(data => new MemoryView.ViewModel(data.Id, data.Image, _memoryRepository.IsUnlock(data.Id)))
                .ToArray();

            return new MemoryViewingCanvas.ViewModel(viewModels);
        }
    }
}