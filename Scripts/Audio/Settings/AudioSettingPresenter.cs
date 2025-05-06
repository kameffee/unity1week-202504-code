using R3;
using Unity1week202504.Audio.Player;
using Unity1week202504.Extensions;
using VContainer.Unity;

namespace Unity1week202504.Audio.Settings
{
    public class AudioSettingPresenter : Presenter, IInitializable
    {
        private readonly AudioSettingView _audioSettingView;
        private readonly AudioSettingsService _audioSettingsService;
        private readonly AudioPlayer _audioPlayer;

        public AudioSettingPresenter(
            AudioSettingView audioSettingView,
            AudioSettingsService audioSettingsService,
            AudioPlayer audioPlayer)
        {
            _audioSettingView = audioSettingView;
            _audioSettingsService = audioSettingsService;
            _audioPlayer = audioPlayer;
        }

        public void Initialize()
        {
            var viewModel = new AudioSettingView.ViewModel(
                _audioSettingsService.BgmVolume.Value,
                _audioSettingsService.SeVolume.Value);
            _audioSettingView.ApplyViewModel(viewModel);

            _audioSettingView.OnChangeBgmVolumeAsObservable()
                .Subscribe(volume => _audioSettingsService.SetBgmVolume(volume))
                .AddTo(this);

            _audioSettingView.OnChangeSeVolumeAsObservable()
                .Subscribe(volume => _audioSettingsService.SetSeVolume(volume))
                .AddTo(this);

            _audioSettingView.OnPointerUpSeVolumeAsObservable()
                .Subscribe(_ => _audioPlayer.PlaySe(_audioSettingView.GetSeSampleClip()))
                .AddTo(this);
        }
    }
}