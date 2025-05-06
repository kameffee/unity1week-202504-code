using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Unity1week202504.Audio
{
    [Serializable]
    public class AudioClipData
    {
        [SerializeField]
        private string _id;

        [SerializeField]
        private AudioResource _audioResource;

        public string Id => _id;
        public AudioResource AudioResource => _audioResource;

        public bool Validate()
            => !string.IsNullOrEmpty(_id) && _audioResource != null;
    }
}