using Unity1week202504.Data.Messages;
using UnityEngine;

namespace Unity1week202504.Data
{
    [CreateAssetMenu(fileName = "MessageBlockMasterDataSource", menuName = "MasterData/MessageBlock/DataSource", order = 0)]
    public class MessageBlockMasterDataSource : ScriptableObject
    {
        [SerializeField]
        private MessageBlockMasterData[] _messageBlockMasterData;

        public MessageBlockMasterData[] All => _messageBlockMasterData;

        public MessageBlockMasterData Get(string key)
        {
            foreach (var data in _messageBlockMasterData)
            {
                if (data.Key == key)
                {
                    return data;
                }
            }

            Debug.LogError($"MessageBlockMasterData not found: {key}");
            return null;
        }
    }
} 