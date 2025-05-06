using UnityEngine;

namespace Unity1week202504.Data.Messages
{
    [CreateAssetMenu(fileName = "MB_xxx", menuName = "MasterData/MessageBlock/Data")]
    public class MessageBlockMasterData : ScriptableObject
    {
        public string Key => _key;
        public MessageBlock MessageBlock => _messageBlock;

        [SerializeField]
        private string _key;

        [Multiline]
        [SerializeField]
        private string _memo;

        [SerializeField]
        private MessageBlock _messageBlock;
    }
}