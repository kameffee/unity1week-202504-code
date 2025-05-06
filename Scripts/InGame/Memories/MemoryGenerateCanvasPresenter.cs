using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Audio.Player;
using Unity1week202504.Data;
using Unity1week202504.Extensions;
using Unity1week202504.Message;
using VContainer.Unity;

namespace Unity1week202504.InGame.Memories
{
    public class MemoryGenerateCanvasPresenter : Presenter, IStartable
    {
        private readonly MemoryFactoryCanvas _view;
        private readonly MemoryViewingCanvasPresenter _memoryViewingCanvasPresenter;
        private readonly MessagePlayer _messagePlayer;
        private readonly PhotoMasterDataSource _photoMasterDataSource;
        private readonly AudioPlayer _audioPlayer;

        public MemoryGenerateCanvasPresenter(
            MemoryFactoryCanvas view,
            MemoryViewingCanvasPresenter memoryViewingCanvasPresenter,
            MessagePlayer messagePlayer,
            PhotoMasterDataSource photoMasterDataSource,
            AudioPlayer audioPlayer)
        {
            _view = view;
            _memoryViewingCanvasPresenter = memoryViewingCanvasPresenter;
            _messagePlayer = messagePlayer;
            _photoMasterDataSource = photoMasterDataSource;
            _audioPlayer = audioPlayer;
        }

        public void Start()
        {
            _view.OnClickMemoryViewAsObservable()
                .SubscribeAwait(async (_, ct) =>
                {
                    // 無効化しておく
                    _view.SetInteractable(false);

                    await UniTask.WhenAll(
                        _audioPlayer.StopBgm(0.2f),
                        _memoryViewingCanvasPresenter.ShowAsync(ct)
                    );

                    _audioPlayer.PlayBgm("MemoryViewing");

                    await _memoryViewingCanvasPresenter.OnCloseAsObservable().FirstAsync(ct);
                    _audioPlayer.PlayBgm("MemoryGeneratePhase/SelectPhoto");
                    _view.SetInteractable(true);
                })
                .AddTo(this);

            _view.OnSelectPhotoAsObservable()
                .SubscribeAwait(async (id, ct) =>
                {
                    var photoMasterData = _photoMasterDataSource.Get(id);
                    await _messagePlayer.PlayAsync(photoMasterData.SelectSpeech, MessageMode.Auto, ct);
                }, AwaitOperation.Switch)
                .AddTo(this);
        }
    }
}