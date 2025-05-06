using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using Unity1week202504.Data.Messages;
using Unity1week202504.InGame.Memories;
using Unity1week202504.InGame.Photo;
using UnityEngine;

namespace Unity1week202504.Data
{
    [HorizontalGroup]
    [CreateAssetMenu(fileName = "MemoryCondition_000", menuName = "MasterData/MemoryCondition/Data")]
    public class MemoryConditionMasterData : ScriptableObject
    {
        public MemoryConditionId Id => new(_id);
        public PhotoPair PhotoPair => new(new PhotoId(_photoId1), new PhotoId(_photoId2));
        public MessageBlock GeneratedComment => _generatedComment;
        public MemoryId OutputMemoryId => new(_outputMemoryId);

        [Min(0)]
        [SerializeField]
        private int _id;

        [Title("Photo IDs")]
        [PhotoId]
        [SerializeField]
        private int _photoId1;

        [PhotoId]
        [SerializeField]
        private int _photoId2;

        [LabelText("生成時のコメント")]
        [SerializeField]
        private MessageBlock _generatedComment;

        [SerializeField]
        private int _outputMemoryId;
    }
}