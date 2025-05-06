using System;
using UnityEngine;

namespace Unity1week202504.Data.Messages
{
    [Serializable]
    public class MessageLine
    {
        public EmotionType EmotionType => _emotionType;
        public string Message => _message;

        [SerializeField]
        private EmotionType _emotionType;

        [Multiline(3)]
        [SerializeField]
        private string _message;
    }
}