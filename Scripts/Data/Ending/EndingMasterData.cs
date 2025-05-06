using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using Unity1week202504.Data.Memories;
using Unity1week202504.Data.Messages;
using Unity1week202504.InGame.Memories;
using UnityEngine;

namespace Unity1week202504.Data.Ending
{
    [CreateAssetMenu(fileName = "EndingMasterData_000", menuName = "MasterData/Ending/Data")]
    public class EndingMasterData : ScriptableObject
    {
        public EndingId Id => new(_id);
        public EndingType Type => _endingType;
        public string EndingName => _endingName;

        // 数値が高いものほど優先度が高い
        public int Priority => ConditionMemoryIds.Count(id => !id.IsEmpty);

        public IEnumerable<MemoryId> ConditionMemoryIds => new[] { _condition1, _condition2, _condition3 }
            .Select(x => new MemoryId(x));

        public AudioClip EndingBGM => _endingBGM;
        public MessageBlock EndingComment => _endingComment;

        [SerializeField]
        private int _id;

        [SerializeField]
        private EndingType _endingType;

        [SerializeField]
        private string _endingName;

        [SerializeField]
        private int _condition1;

        [SerializeField]
        private int _condition2;

        [SerializeField]
        private int _condition3;

        [SerializeField]
        private AudioClip _endingBGM;

        [SerializeField]
        private MessageBlock _endingComment;
    }
}