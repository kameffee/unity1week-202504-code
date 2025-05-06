using Unity1week202504.Data.Messages;
using Unity1week202504.InGame.Memories;
using UnityEngine;

namespace Unity1week202504.Data.Memories
{
    [CreateAssetMenu(fileName = "Memory_000", menuName = "MasterData/Memory/Data", order = 0)]
    public class MemoryMasterData : ScriptableObject
    {
        public MemoryId Id => new(_id);
        public string DisplayName => _displayName;
        public Sprite Image => _image;
        public MessageBlock ViewingMessage => _viewingMessage;

        [SerializeField]
        private int _id;

        [SerializeField]
        private string _displayName;

        [SerializeField]
        private Sprite _image;

        [SerializeField]
        private MessageBlock _viewingMessage;
    }
}