using Alchemy.Inspector;
using Unity1week202504.Data.Messages;
using Unity1week202504.InGame;
using Unity1week202504.InGame.Photo;
using UnityEngine;

namespace Unity1week202504.Data
{
    [CreateAssetMenu(fileName = "PhotoMasterData_000", menuName = "MasterData/Photo/Data", order = 0)]
    public class PhotoMasterData : ScriptableObject
    {
        public PhotoId Id => new(_id);
        public string PhotoName => _photoName;
        public TimeOfDay ShowTimeOfDay => _showTimeOfDay;
        public Sprite Sprite => _sprite;
        public MessageBlock FocusSpeech => _focusSpeech;
        public MessageBlock ShutterSpeech => _shutterSpeech;
        public MessageBlock SelectSpeech => _selectSpeech;
        public bool IsDisappearableAfterSnap => _isDisappearableAfterSnap;

        [Min(0)]
        [SerializeField]
        private int _id;

        [SerializeField]
        private string _photoName;

        [SerializeField]
        private TimeOfDay _showTimeOfDay;

        [Preview]
        [SerializeField]
        private Sprite _sprite;

        [SerializeField]
        [LabelText("撮影前のセリフ")]
        private MessageBlock _focusSpeech;
        
        [SerializeField]
        [LabelText("撮影後のセリフ")]
        private MessageBlock _shutterSpeech;

        [SerializeField]
        [LabelText("選択時のセリフ")]
        private MessageBlock _selectSpeech;
        
        [SerializeField]
        [LabelText("槍が消えるフラグ")]
        private bool _isDisappearableAfterSnap;

        public bool IsShowable(TimeOfDay timeOfDay)
        {
            return _showTimeOfDay.HasFlag(timeOfDay);
        }
    }
}