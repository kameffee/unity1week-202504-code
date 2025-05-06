using Unity1week202504.InGame.Memories;
using UnityEngine;

namespace Unity1week202504.Data
{
    [CreateAssetMenu(fileName = "MemoryConditionMasterDataSource", menuName = "MasterData/MemoryCondition/DataSource", order = 0)]
    public class MemoryConditionMasterDataSource : ScriptableObject
    {
        [SerializeField]
        private MemoryConditionMasterData[] _memoryConditionMasterData;

        public MemoryConditionMasterData[] All => _memoryConditionMasterData;

        public MemoryConditionMasterData Get(MemoryConditionId id)
        {
            foreach (var data in _memoryConditionMasterData)
            {
                if (data.Id == id)
                {
                    return data;
                }
            }

            Debug.LogError($"MemoryConditionMasterData not found: {id}");
            return null;
        }
    }
} 