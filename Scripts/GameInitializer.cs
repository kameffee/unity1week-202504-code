using System.Linq;
using Unity1week202504.Data.Memories;
using Unity1week202504.InGame.Memories;
using UnityEngine;
using VContainer.Unity;

namespace Unity1week202504
{
    public class GameInitializer : IInitializable
    {
        private readonly MemorySettings _memorySettings;
        private readonly MemoryRepository _memoryRepository;

        public GameInitializer(
            MemorySettings memorySettings,
            MemoryRepository memoryRepository)
        {
            _memorySettings = memorySettings;
            _memoryRepository = memoryRepository;
        }

        public void Initialize()
        {
            Debug.Log("GameInitializer.Initialize");
            
            // デフォルトのメモリーを追加
            _memoryRepository.Add(_memorySettings.DefaultMemories.Select(data => data.Id).ToArray());
        }
    }
}