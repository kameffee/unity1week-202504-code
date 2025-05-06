using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Extensions;
using VContainer.Unity;

namespace Unity1week202504.License
{
    public class LicensePresenter : Presenter, IInitializable
    {
        private readonly LicenseView _view;
        private readonly LicenseTextProvider _licenseTextProvider;

        public LicensePresenter(
            LicenseView view,
            LicenseTextProvider licenseTextProvider)
        {
            _view = view;
            _licenseTextProvider = licenseTextProvider;
        }

        public void Initialize()
        {
            _licenseTextProvider.GetAsync().ToObservable().ToObservable()
                .Subscribe(licenseText => _view.SetLicenseText(licenseText))
                .AddTo(this);

            _view.OnCloseAsObservable()
                .Subscribe(_ => _view.HideAsync().Forget())
                .AddTo(this);
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            await _view.ShowAsync(cancellationToken);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            await _view.HideAsync(cancellationToken);
        }
    }
}