using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Audio;

namespace Unity1week202504.Audio.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class BgmPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;

        private float _volume;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        public void Play(AudioClip audioClip, bool isLoop)
        {
            SetVolume(_volume);
            _audioSource.clip = audioClip;
            _audioSource.loop = isLoop;
            _audioSource.Play();
        }

        public void Play(AudioResource audioResource, bool isLoop)
        {
            SetVolume(_volume);
            _audioSource.resource = audioResource;
            _audioSource.loop = isLoop;
            _audioSource.Play();
        }

        public async UniTask StopAsync(float fadeoutTime = 0f)
        {
            if (fadeoutTime > 0f)
            {
                await LMotion.Create(_audioSource.volume, 0f, fadeoutTime)
                    .BindToVolume(_audioSource)
                    .AddTo(this);
            }

            _audioSource.Stop();
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
            _audioSource.volume = volume;
        }
    }
}