using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Audio.Player;
using Unity1week202504.Credit;
using Unity1week202504.Extensions;
using Unity1week202504.License;
using VContainer.Unity;
using Unity1week202504.Scenes;

namespace Unity1week202504.Title
{
    public class TitlePresenter : Presenter, IInitializable, IStartable
    {
        private readonly MenuView _menuView;
        private readonly LicensePresenter _licensePresenter;
        private readonly SceneLoader _sceneLoader;
        private readonly AudioPlayer _audioPlayer;
        private readonly CreditPresenter _creditPresenter;

        public TitlePresenter(
            MenuView menuView,
            LicensePresenter licensePresenter,
            SceneLoader sceneLoader,
            AudioPlayer audioPlayer,
            CreditPresenter creditPresenter)
        {
            _menuView = menuView;
            _licensePresenter = licensePresenter;
            _sceneLoader = sceneLoader;
            _audioPlayer = audioPlayer;
            _creditPresenter = creditPresenter;
        }

        public void Initialize()
        {
            _menuView.OnClickStartAsObservable()
                .Subscribe(_ => TransitionToInGame())
                .AddTo(this);

            _menuView.OnClickLicenseAsObservable()
                .SubscribeAwait(async (_, ct) => await ShowLicenseAsync(ct), AwaitOperation.Drop)
                .AddTo(this);

            _menuView.OnClickCreditAsObservable()
                .SubscribeAwait(async (_, ct) => await ShowCreditAsync(ct), AwaitOperation.Drop)
                .AddTo(this);
        }

        public void Start()
        {
            _audioPlayer.PlayBgm("Title");
        }

        private async UniTask ShowLicenseAsync(CancellationToken cancellationToken)
        {
            await _licensePresenter.ShowAsync(cancellationToken);
        }

        private async UniTask ShowCreditAsync(CancellationToken cancellationToken)
        {
            await _creditPresenter.ShowAsync(cancellationToken);
        }

        private void TransitionToInGame()
        {
            _sceneLoader.LoadAsync("InGame").Forget();
        }
    }
}