using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Audio.Player;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using VContainer;

namespace Unity1week202504.InGame.Memories
{
    public class MemoryViewingCanvas : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [Title("Memory Image")]
        [SerializeField]
        private Image _memoryImage;

        [SerializeField]
        private Sprite _defaultMemoryImage;

        [Title("Prefab")]
        [SerializeField]
        private MemoryView _memoryViewPrefab;

        [SerializeField]
        private Transform _memoryViewParent;

        [Title("Button")]
        [SerializeField]
        private Button _closeButton;

        [Title("Audio")]
        [SerializeField]
        private AudioResource _openSe;

        private readonly List<MemoryView> _memoryViews = new();
        private readonly Subject<MemoryId> _onClickMemory = new();

        private AudioPlayer _audioPlayer;
        private ViewModel _viewModel;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            // 既存のMemoryViewを削除
            foreach (Transform child in _memoryViewParent)
            {
                Destroy(child.gameObject);
            }

            _onClickMemory
                .Subscribe(SetSelectedMemory)
                .AddTo(this);
        }

        [Inject]
        public void Construct(AudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
        }

        public Observable<Unit> OnClickCloseAsObservable() => _closeButton.OnClickAsObservable();
        public Observable<MemoryId> OnSelectedMemoryAsObservable() => _onClickMemory;

        public void ApplyViewModel(ViewModel viewModel)
        {
            _viewModel = viewModel;

            // 既存のMemoryViewを削除
            foreach (Transform child in _memoryViewParent)
            {
                Destroy(child.gameObject);
            }

            _memoryViews.Clear();

            foreach (var memoryViewModel in viewModel.MemoryViewModels)
            {
                var memoryView = Instantiate(_memoryViewPrefab, _memoryViewParent);
                memoryView.Apply(memoryViewModel);
                memoryView.OnClickAsObservable()
                    .Where(_ => memoryViewModel.IsUnlocked)
                    .Subscribe(id => _onClickMemory.OnNext(id))
                    .AddTo(memoryView);

                // 追加
                _memoryViews.Add(memoryView);
            }

            SetUnselectedAll();
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            UpdateMainMemoryImage(MemoryId.Empty);
            SetUnselectedAll();

            _audioPlayer.PlaySe(_openSe);

            await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: cancellationToken);

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

        private void SetSelectedMemory(MemoryId memoryId)
        {
            // 未開放時は何もしない
            if (_viewModel.MemoryViewModels.First(x => x.MemoryId == memoryId).IsLocked)
            {
                return;
            }

            // 画像更新
            UpdateMainMemoryImage(memoryId);

            // 各ボタンの選択状態を更新
            foreach (var memoryView in _memoryViews)
            {
                var isSelected = memoryView.MemoryId == memoryId;
                memoryView.SetSelected(isSelected);
            }
        }

        private void UpdateMainMemoryImage(MemoryId memoryId)
        {
            if (memoryId.IsEmpty)
            {
                // 何も選択されていない場合は、デフォルトの画像を表示
                _memoryImage.sprite = _defaultMemoryImage;
                return;
            }

            var memoryModel = _viewModel.MemoryViewModels
                .FirstOrDefault(x => x.MemoryId == memoryId);
            _memoryImage.sprite = memoryModel.MemoryImage;
        }

        private void SetUnselectedAll()
        {
            foreach (var memoryView in _memoryViews)
            {
                memoryView.SetSelected(false);
            }
        }

        public class ViewModel
        {
            public MemoryView.ViewModel[] MemoryViewModels { get; }

            public ViewModel(MemoryView.ViewModel[] memoryViewModels)
            {
                MemoryViewModels = memoryViewModels;
            }
        }
    }
}