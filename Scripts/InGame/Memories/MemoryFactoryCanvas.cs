using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Audio.Player;
using Unity1week202504.InGame.Photo;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using VContainer;

namespace Unity1week202504.InGame.Memories
{
    public class MemoryFactoryCanvas : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Button _submitButton;

        [Title("Prefab")]
        [SerializeField]
        private PhotoView _photoViewPrefab;

        [SerializeField]
        private Transform _photoViewParent;

        [Title("Selected Photo")]
        [SerializeField]
        private Image _selectedPhoto1;

        [SerializeField]
        private Image _selectedPhoto2;

        [Title("Memory")]
        [SerializeField]
        private Button _memoryViewButton;

        [SerializeField]
        private AudioResource _openSe;

        private readonly List<PhotoView> _photoViews = new();
        private readonly Subject<PhotoId> _photoClickSubject = new();
        private readonly List<PhotoId> _selectedPhotoIds = new();
        private readonly Subject<PhotoId> _onSelectPhoto = new();

        private AudioPlayer _audioPlayer;
        private ViewModel _viewModel;
        private MemoryGenerateHistoryRepository _memoryGenerateHistoryRepository;

        [Inject]
        public void Construct(
            MemoryGenerateHistoryRepository memoryGenerateHistoryRepository,
            AudioPlayer audioPlayer)
        {
            _memoryGenerateHistoryRepository = memoryGenerateHistoryRepository;
            _audioPlayer = audioPlayer;
        }

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _selectedPhoto1.gameObject.SetActive(false);
            _selectedPhoto2.gameObject.SetActive(false);

            // 既存のPhotoViewを削除
            foreach (Transform child in _photoViewParent)
            {
                Destroy(child.gameObject);
            }

            OnClickPhotoAsObservable()
                .Subscribe(OnClickPhotoView)
                .AddTo(this);
        }

        private void OnClickPhotoView(PhotoId id)
        {
            var photoView = _photoViews.Find(view => view.PhotoId == id);
            if (_selectedPhotoIds.Contains(id))
            {
                // 選択解除
                _selectedPhotoIds.Remove(id);
                photoView.SetSelected(false);

                UpdateSelectedPhotos();
                return;
            }

            if (_selectedPhotoIds.Count >= 2)
                return;

            // 選択する
            photoView.SetSelected(true);
            _selectedPhotoIds.Add(id);
            _onSelectPhoto.OnNext(id);

            UpdateSelectedPhotos();
        }

        private void UpdateSelectedPhotos()
        {
            var selectedPhotos = _selectedPhotoIds
                .Select(id => _viewModel.GetSprite(id))
                .ToArray();

            if (selectedPhotos.Length > 0)
            {
                _selectedPhoto1.sprite = selectedPhotos[0];
                _selectedPhoto1.gameObject.SetActive(true);
            }
            else
            {
                _selectedPhoto1.sprite = null;
                _selectedPhoto1.gameObject.SetActive(false);
            }

            if (selectedPhotos.Length > 1)
            {
                _selectedPhoto2.sprite = selectedPhotos[1];
                _selectedPhoto2.gameObject.SetActive(true);
            }
            else
            {
                _selectedPhoto2.sprite = null;
                _selectedPhoto2.gameObject.SetActive(false);
            }

            _submitButton.interactable = IsGeneratable();
            UpdateAlreadyGeneratedPhoto();
        }

        // すでに生成済みをチェック表示する
        private void UpdateAlreadyGeneratedPhoto()
        {
            // １つ目も選択されていなければ全部オフ
            if (_selectedPhotoIds.Count == 0)
            {
                _photoViews.ForEach(view => view.SetGeneratedMark(false));
                return;
            }

            foreach (var photoView in _photoViews)
            {
                var photoPair = new PhotoPair(_selectedPhotoIds[0], photoView.PhotoId);
                var isGeneratedPair = _memoryGenerateHistoryRepository.Contains(photoPair);
                photoView.SetGeneratedMark(isGeneratedPair && !photoView.IsBlank);
            }
        }

        private bool IsGeneratable()
        {
            return _selectedPhotoIds.Count == 2;
        }

        public void ApplyViewModel(ViewModel viewModel)
        {
            _viewModel = viewModel;

            _photoViews.ForEach(x => Destroy(x.gameObject));
            _photoViews.Clear();

            foreach (var viewModelPhotoView in viewModel.PhotoViewModels)
            {
                var photoView = Instantiate(_photoViewPrefab, _photoViewParent);
                _photoViews.Add(photoView);
                photoView.OnClickAsObservable()
                    .Subscribe(photoId => _photoClickSubject.OnNext(photoId))
                    .AddTo(photoView);

                photoView.Apply(viewModelPhotoView);
            }

            Reset();
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            _audioPlayer.PlaySe(_openSe);

            await UniTask.Delay(TimeSpan.FromSeconds(1.2f), cancellationToken: cancellationToken);

            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        
        public void SetInteractable(bool interactable)
        {
            _canvasGroup.interactable = interactable;
        }

        public Observable<PhotoId> OnClickPhotoAsObservable() => _photoClickSubject;
        public Observable<PhotoId> OnSelectPhotoAsObservable() => _onSelectPhoto;

        public Observable<(PhotoId photoId1, PhotoId photoId2)> OnSubmitAsObservable() => _submitButton
            .OnClickAsObservable()
            .Select(_ => (_selectedPhotoIds[0], _selectedPhotoIds[1]));

        public Observable<Unit> OnClickMemoryViewAsObservable() => _memoryViewButton.OnClickAsObservable();

        public class ViewModel
        {
            public PhotoView.ViewModel[] PhotoViewModels { get; }

            public ViewModel(PhotoView.ViewModel[] photoViewModels)
            {
                PhotoViewModels = photoViewModels;
            }

            public Sprite GetSprite(PhotoId photoId)
            {
                var photoViewModel = PhotoViewModels.FirstOrDefault(x => x.PhotoId == photoId);
                if (photoViewModel != null)
                {
                    return photoViewModel.PhotoImage;
                }

                Debug.LogError($"PhotoId {photoId} not found.");
                return null;
            }
        }

        public void Reset()
        {
            _photoViews.ForEach(view => view.SetSelected(false));
            _selectedPhotoIds.Clear();
            _selectedPhoto1.sprite = null;
            _selectedPhoto2.sprite = null;
            UpdateSelectedPhotos();
        }
    }
}