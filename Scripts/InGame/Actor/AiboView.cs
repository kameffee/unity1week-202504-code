using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using Unity1week202504.Data.Messages;
using UnityEngine;

namespace Unity1week202504.InGame.Actor
{
    public class AiboView : MonoBehaviour
    {
        [Serializable]
        public class Pair
        {
            public EmotionType EmotionType => _emotionType;
            public GameObject EmoteObject => _emoteObject;

            [SerializeField]
            private EmotionType _emotionType;

            [SerializeField]
            private GameObject _emoteObject;
        }

        [ValidateInput(nameof(Validate), "Emotionタイプが重複しています")]
        [SerializeField]
        private List<Pair> _pairs;

        private void Awake()
        {
            SetDefaultEmotion();
        }

        public void ChangeEmotion(EmotionType emotionType)
        {
            foreach (var pair in _pairs)
            {
                pair.EmoteObject.SetActive(pair.EmotionType == emotionType);
            }
        }
        
        public void SetDefaultEmotion()
        {
            ChangeEmotion(EmotionType.None);
        }

        private bool Validate(List<Pair> pairs)
        {
            var duplicateDatas = pairs
                .GroupBy(pair => pair.EmotionType)
                .Where(group => group.Count() > 1);

            return !duplicateDatas.Any();
        }
    }
}