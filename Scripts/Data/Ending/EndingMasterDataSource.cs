using Unity1week202504.Data.Ending;
using UnityEngine;

namespace Unity1week202504.Data
{
    [CreateAssetMenu(fileName = "EndingMasterDataSource", menuName = "MasterData/Ending/DataSource", order = 0)]
    public class EndingMasterDataSource : ScriptableObject
    {
        [SerializeField]
        private EndingMasterData[] _endingMasterData;

        public EndingMasterData[] All => _endingMasterData;

        public EndingMasterData Get(EndingId id)
        {
            foreach (var data in _endingMasterData)
            {
                if (data.Id == id)
                {
                    return data;
                }
            }

            Debug.LogError($"EndingMasterData not found: {id}");
            return null;
        }
    }
} 