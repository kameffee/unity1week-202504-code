using Alchemy.Inspector;
using UnityEngine;

namespace Unity1week202504.Data
{
    [CreateAssetMenu(fileName = "DebugSettings", menuName = "DebugSettings", order = 0)]
    public class DebugSettings : ScriptableObject
    {
        public bool IsDebug => _isDebug;
        public bool IsAllMemoryUnlocked => _isDebug && _isAllMemoryUnlocked;

        [LabelText("デバッグモードの有効")]
        [SerializeField]
        private bool _isDebug;

        [LabelText("思い出全開放")]
        [SerializeField]
        private bool _isAllMemoryUnlocked;
    }
}