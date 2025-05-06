using Alchemy.Inspector;
using Unity1week202504.Audio.Settings;
using Unity1week202504.Credit;
using Unity1week202504.License;
using Unity1week202504.Title;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Unity1week202504.Installer
{
    public class TitleLifetimeScope : LifetimeScope
    {
        [Title("License")]
        [SerializeField]
        private TextAsset _licenseTextAsset;

        [SerializeField]
        private LicenseView _licenseViewPrefab;

        [Title("Credit")]
        [SerializeField]
        private CreditView _creditViewPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<TitlePresenter>();
            builder.RegisterComponentInHierarchy<MenuView>();

            ConfigureLicense(builder);
            ConfigureAudio(builder);
            ConfigureCredit(builder);
        }

        private void ConfigureLicense(IContainerBuilder builder)
        {
            builder.Register<LicenseTextProvider>(Lifetime.Singleton).WithParameter(_licenseTextAsset);
            builder.RegisterComponentInNewPrefab<LicenseView>(_licenseViewPrefab, Lifetime.Singleton);
            builder.RegisterEntryPoint<LicensePresenter>().AsSelf();
        }

        private void ConfigureAudio(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<BgmSettingView>();
            builder.RegisterComponentInHierarchy<SeSettingView>();
            builder.RegisterComponentInHierarchy<AudioSettingView>();
            builder.RegisterEntryPoint<AudioSettingPresenter>();
        }

        private void ConfigureCredit(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab<CreditView>(_creditViewPrefab, Lifetime.Singleton);
            builder.RegisterEntryPoint<CreditPresenter>().AsSelf();
        }
    }
}