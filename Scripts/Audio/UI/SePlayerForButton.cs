using Unity1week202504.Audio.Player;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Unity1week202504.Audio
{
    public class SePlayerForButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private AudioResource _onHoverSe;
        
        [SerializeField]
        private AudioResource _onClickSe;

        [Inject]
        private readonly AudioPlayer _audioPlayer;

        private void Start()
        {
            var lifetimeScope = LifetimeScope.Find<LifetimeScope>();
            lifetimeScope.Container.Inject(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_button.interactable) return;

            if (_onHoverSe == null)
            {
                Debug.LogWarning("AudioResourceがnullです。", gameObject);
                return;
            }

            _audioPlayer.PlaySe(_onHoverSe);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_button.interactable) return;

            if (_onClickSe == null)
            {
                Debug.LogWarning("AudioResourceがnullです。", gameObject);
                return;
            }

            _audioPlayer.PlaySe(_onClickSe);
        }

        private void OnValidate()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }

            Assert.IsNotNull(_button, "Buttonがnullです。");
        }
    }
}