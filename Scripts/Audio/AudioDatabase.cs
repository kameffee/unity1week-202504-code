using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unity1week202504.Audio
{
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = "Audio/AudioDatabase")]
    public class AudioDatabase : ScriptableObject
    {
        [SerializeField]
        private AudioClipData[] _bgmAudioDatas;

        [SerializeField]
        private AudioClipData[] _seAudioDatas;

        public AudioClipData Get(AudioType audioType, string id)
        {
            return audioType switch
            {
                AudioType.Bgm => GetBgm(id),
                AudioType.Se => GetSe(id),
                _ => throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null)
            };
        }

        private AudioClipData GetSe(string id)
        {
            var data = _seAudioDatas.FirstOrDefault(data => data.Id.Equals(id));
            if (data == null)
            {
                throw new FileNotFoundException($"AudioClip is not found. id: {id}");
            }

            return data;
        }

        private AudioClipData GetBgm(string id)
        {
            var data = _bgmAudioDatas.FirstOrDefault(data => data.Id.Equals(id));
            if (data == null)
            {
                throw new FileNotFoundException($"AudioClip is not found. id: {id}");
            }

            return data;
        }

        public bool Validate()
        {
            var sb = new StringBuilder();
            var isValid = true;

            foreach (var data in _bgmAudioDatas)
            {
                if (!data.Validate())
                {
                    sb.AppendLine($"BGM AudioClipData is invalid. id: {data.Id}");
                    isValid = false;
                }
            }

            foreach (var data in _seAudioDatas)
            {
                if (!data.Validate())
                {
                    sb.AppendLine($"SE AudioClipData is invalid. id: {data.Id}");
                    isValid = false;
                }
            }

            if (!isValid)
            {
                Debug.LogError(sb.ToString());
            }

            return isValid;
        }
    }
}