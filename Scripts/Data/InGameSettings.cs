using System;
using System.Collections.Generic;
using Unity1week202504.InGame;
using UnityEngine;

namespace Unity1week202504.Data
{
    [CreateAssetMenu(fileName = "InGameSettings", menuName = "Settings/InGame", order = 0)]
    public class InGameSettings : ScriptableObject
    {
        [Serializable]
        public class MemoryGenerateSettings
        {
            public TimeOfDay TimeOfDay => _timeOfDay;
            public int Count => _count;

            [SerializeField]
            private TimeOfDay _timeOfDay;

            [SerializeField]
            private int _count;
        }
        
        public int GetMemoryGenerateCount(TimeOfDay timeOfDay)
        {
            return _memoryGenerateSettings.Find(settings => settings.TimeOfDay == timeOfDay).Count;
        }

        [SerializeField]
        private List<MemoryGenerateSettings> _memoryGenerateSettings = new();
        
    }
}