using System;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Audio.Settings;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using VContainer.Unity;

namespace Unity1week202504.Audio.Player
{
    public class AudioPlayer : IInitializable, IDisposable
    {
        private readonly BgmPlayer _bgmPlayer;
        private readonly SePlayer _sePlayer;
        private readonly AudioSettingsService _audioSettingsService;
        private readonly AudioDatabase _audioDatabase;
        private readonly CompositeDisposable _disposable = new();

        public AudioPlayer(
            AudioSettingsService audioSettingsService,
            BgmPlayer bgmPlayer,
            SePlayer sePlayerPlayer,
            AudioDatabase audioDatabase)
        {
            _bgmPlayer = bgmPlayer;
            _sePlayer = sePlayerPlayer;
            _audioSettingsService = audioSettingsService;
            _audioDatabase = audioDatabase;
        }

        public void Initialize()
        {
            Assert.IsTrue(_audioDatabase.Validate(), "AudioDatabase is not valid.");

            _audioSettingsService.BgmVolume
                .Subscribe(volume => _bgmPlayer.SetVolume(volume.Value))
                .AddTo(_disposable);

            _audioSettingsService.SeVolume
                .Subscribe(volume => _sePlayer.SetVolume(volume.Value))
                .AddTo(_disposable);
        }

        public void PlayBgm(AudioClip clip, bool isLoop = true)
        {
            _bgmPlayer.Play(clip, isLoop);
        }

        public void PlayBgm(string id, bool isLoop = true)
        {
            var audioClipData = _audioDatabase.Get(AudioType.Bgm, id);
            _bgmPlayer.Play(audioClipData.AudioResource, isLoop);
        }

        public async UniTask StopBgm(float fadeOutDuration = 1f)
        {
            await _bgmPlayer.StopAsync(fadeOutDuration);
        }

        public void PlaySe(AudioClip clip)
        {
            _sePlayer.PlayOneShot(clip);
        }

        public void PlaySe(string id)
        {
            var audioClipData = _audioDatabase.Get(AudioType.Se, id);
            _sePlayer.PlayOneShot(audioClipData.AudioResource);
        }

        public void PlaySe(AudioResource audioResource)
        {
            _sePlayer.PlayOneShot(audioResource);
        }

        public void StopSe() => _sePlayer.Stop();

        public void Dispose() => _disposable.Dispose();
    }
}