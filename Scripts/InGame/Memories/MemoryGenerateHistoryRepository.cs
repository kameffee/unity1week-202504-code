using System.Collections.Generic;

namespace Unity1week202504.InGame.Memories
{
    public class MemoryGenerateHistoryRepository
    {
        private readonly HashSet<PhotoPair> _photoPairs = new();
        public void Add(PhotoPair pair) => _photoPairs.Add(pair);
        public bool Contains(PhotoPair pair) => _photoPairs.Contains(pair);
    }
}