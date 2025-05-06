using System.Collections.Generic;
using UnityEngine;

namespace Unity1week202504.InGame.Memories
{
    public class MemoryRepository
    {
        private readonly HashSet<MemoryId> _memories = new();

        public void Add(MemoryId memoryId)
        {
            Debug.Log($"MemoryRepository.Add: {memoryId}");
            _memories.Add(memoryId);
        }

        public void Add(params MemoryId[] memoryIds)
        {
            foreach (var memoryId in memoryIds)
                Add(memoryId);
        }

        public bool IsUnlock(MemoryId memoryId) => _memories.Contains(memoryId);
    }
}