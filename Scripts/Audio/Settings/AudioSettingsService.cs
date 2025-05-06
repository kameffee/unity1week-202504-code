using R3;
using Unity1week202504.Audio.System;

namespace Unity1week202504.Audio.Settings
{
    public class AudioSettingsService
    {
        public ReactiveProperty<AudioVolume> BgmVolume => _bgmVolume;
        public ReactiveProperty<AudioVolume> SeVolume => _seVolume;

        private readonly ReactiveProperty<AudioVolume> _bgmVolume = new(new AudioVolume(DefaultBgmVolume));
        private readonly ReactiveProperty<AudioVolume> _seVolume = new(new AudioVolume(DefaultSeVolume));

        private const float DefaultBgmVolume = 0.3f;
        private const float DefaultSeVolume = 0.6f;

        public void SetBgmVolume(AudioVolume volume) => _bgmVolume.Value = volume;

        public void SetSeVolume(AudioVolume volume) => _seVolume.Value = volume;
    }
}