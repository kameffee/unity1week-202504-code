using System;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using Unity1week202504.Audio.Player;
using Unity1week202504.Data;
using Unity1week202504.InGame.Photo;
using Unity1week202504.Message;
using UnityEngine;
using VContainer;

namespace Unity1week202504.InGame
{
    public class SnapTargetObject : MonoBehaviour
    {
        public PhotoId PhotoId => _photoMasterData.Id;
        public bool Snappable => _isSnappable;
        public Vector3 WorldPosition => transform.position;

        [ReadOnly]
        [ShowInInspector]
        private bool _isSnappable = true;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [InlineEditor]
        [Required]
        [SerializeField]
        private PhotoMasterData _photoMasterData;

        private MessagePlayer _messagePlayer;
        private AudioPlayer _audioPlayer;

        [Inject]
        public void Construct(
            MessagePlayer messagePlayer,
            AudioPlayer audioPlayer)
        {
            _messagePlayer = messagePlayer;
            _audioPlayer = audioPlayer;
        }

        public void Snap()
        {
            if (!_isSnappable) return;

            Debug.Log($"Snap: {_photoMasterData.PhotoName}", gameObject);

            _audioPlayer.PlaySe("TakePhotoPhase/TakePhotoShutter");
            if (_photoMasterData.IsDisappearableAfterSnap)
            {
                // 撮影後に消える
                gameObject.SetActive(false);
            }

            _messagePlayer.PlayAsync(_photoMasterData.ShutterSpeech, MessageMode.Auto).Forget();
        }

        public void SetSnappable(bool isSnappable)
        {
            _isSnappable = isSnappable;
        }

        public bool IsShowable(TimeOfDay timeOfDay)
        {
            return _photoMasterData.IsShowable(timeOfDay);
        }

        public void SetVisible(bool isShowable)
        {
            gameObject.SetActive(isShowable);
        }
    }
}