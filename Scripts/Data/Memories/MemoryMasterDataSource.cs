using Unity1week202504.InGame.Memories;
using UnityEngine;

namespace Unity1week202504.Data.Memories
{
    [CreateAssetMenu(fileName = "MemoryMasterDataSource", menuName = "MasterData/Memory/DataSource", order = 0)]
    public class MemoryMasterDataSource : ScriptableObject
    {
        [SerializeField]
        private MemoryMasterData[] _memoryMasterData;

        public MemoryMasterData[] All => _memoryMasterData;

        public MemoryMasterData Get(MemoryId id)
        {
            foreach (var data in _memoryMasterData)
            {
                if (data.Id == id)
                {
                    return data;
                }
            }

            Debug.LogError($"MemoryMasterData not found: {id}");
            return null;
        }
    }
} 