using System.Collections.Generic;
using UnityEngine;

namespace Unity1week202504.Data.Memories
{
    [CreateAssetMenu(fileName = "MemorySettings", menuName = "Settings/MemorySettings", order = 0)]
    public class MemorySettings : ScriptableObject
    {
        public IReadOnlyList<MemoryMasterData> DefaultMemories => _defaultMemories;

        [SerializeField]
        private List<MemoryMasterData> _defaultMemories;
    }
}