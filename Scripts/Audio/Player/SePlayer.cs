using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Unity1week202504.Audio.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class SePlayer : MonoBehaviour
    {
        private readonly List<AudioSource> _audioSource = new();

        private void Awake()
        {
            var audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            _audioSource.Add(audioSource);
        }

        public void PlayOneShot(AudioClip audioClip)
        {
            var audioSource = GetReadyAudioSource();
            audioSource.PlayOneShot(audioClip);
        }

        public void PlayOneShot(AudioResource audioResource)
        {
            var audioSource = GetReadyAudioSource();
            audioSource.resource = audioResource;
            audioSource.Play();
        }

        public void Stop()
        {
            foreach (var source in _audioSource)
            {
                if (source.isPlaying)
                {
                    source.Stop();
                }
            }
        }

        public void SetVolume(float volume)
        {
            foreach (var source in _audioSource)
            {
                source.volume = volume;
            }
        }

        private AudioSource GetReadyAudioSource()
        {
            foreach (var source in _audioSource)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // すべてのAudioSourceが再生中の場合、新しいAudioSourceを追加して返す
            var newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            newSource.loop = false;
            _audioSource.Add(newSource);

            return newSource;
        }
    }
}