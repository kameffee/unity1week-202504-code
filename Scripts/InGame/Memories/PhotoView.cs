using System;
using Alchemy.Inspector;
using R3;
using R3.Triggers;
using Unity1week202504.Audio.Player;
using Unity1week202504.InGame.Photo;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Unity1week202504.InGame.Memories
{
    public class PhotoView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _root;

        [SerializeField]
        private GameObject _blankRoot;

        [SerializeField]
        private Image _photoImage;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private GameObject _selectedFrame;

        [SerializeField]
        private GameObject _generatedMark;

        [SerializeField]
        private AudioResource _onHoverSe;

        [SerializeField]
        private AudioResource _onClickSe;

        [ShowInInspector]
        public PhotoId PhotoId { get; private set; }

        public bool IsBlank { get; private set; }

        private AudioPlayer _audioPlayer;

        private void Awake()
        {
            var lifetimeScope = LifetimeScope.Find<LifetimeScope>();
            lifetimeScope.Container.Inject(this);

            _selectedFrame.SetActive(false);

            _button.OnPointerEnterAsObservable()
                .Subscribe(_ => _audioPlayer.PlaySe(_onHoverSe))
                .AddTo(this);

            OnClickAsObservable()
                .Subscribe(_ => _audioPlayer.PlaySe(_onClickSe))
                .AddTo(this);
        }

        [Inject]
        public void Construct(AudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
        }

        public void Apply(ViewModel viewModel)
        {
            PhotoId = viewModel.PhotoId;
            IsBlank = !viewModel.HasPhoto;

            _photoImage.sprite = viewModel.PhotoImage;
            _root.SetActive(viewModel.HasPhoto);
            _blankRoot.SetActive(!viewModel.HasPhoto);
        }

        public Observable<PhotoId> OnClickAsObservable() => _button.OnClickAsObservable().Select(_ => PhotoId);

        public class ViewModel
        {
            public PhotoId PhotoId { get; }
            public Sprite PhotoImage { get; }
            public bool HasPhoto { get; }

            public ViewModel(PhotoId id, Sprite photoImage, bool hasPhoto)
            {
                PhotoId = id;
                PhotoImage = photoImage;
                HasPhoto = hasPhoto;
            }
        }

        public void SetSelected(bool selected) => _selectedFrame.SetActive(selected);

        public void SetGeneratedMark(bool visible) => _generatedMark.SetActive(visible);
    }
}