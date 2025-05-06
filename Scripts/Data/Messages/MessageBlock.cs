using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;

namespace Unity1week202504.Data.Messages
{
    [Serializable]
    public class MessageBlock
    {
        public IReadOnlyList<MessageLine> Lines => _lines;

        [ListViewSettings(ShowFoldoutHeader = false)]
        [SerializeField]
        private List<MessageLine> _lines = new();

        public bool IsValid => _lines.Count > 0;
    }
}