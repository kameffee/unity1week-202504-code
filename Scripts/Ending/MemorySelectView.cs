using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using Unity1week202504.InGame.Memories;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202504.Ending
{
    public class MemorySelectView : MonoBehaviour
    {
        public IReadOnlyCollection<MemoryId> SelectedMemoryIds => _selectedMemoryIds;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private List<SelectedMemoryView> _selectedMemoryViews;

        [SerializeField]
        private MemoryView _memoryViewPrefab;

        [SerializeField]
        private Transform _memoryViewParent;

        [SerializeField]
        private Button _submitButton;

        private readonly List<MemoryView> _memoryViews = new();
        private readonly Subject<MemoryId> _onClickMemory = new();
        private readonly Subject<MemoryId> _onSelectedMemory = new();
        private readonly HashSet<MemoryId> _selectedMemoryIds = new();
        private const int MaxSelectableCount = 3;
        private ViewModel _viewModel;

        public Observable<Unit> OnClickSubmitAsObservable() => _submitButton.OnClickAsObservable();
        private Observable<MemoryId> OnClickMemoryAsObservable() => _onClickMemory;
        public Observable<MemoryId> OnSelectedMemoryAsObservable() => _onSelectedMemory;

        private void Awake()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _selectedMemoryViews.ForEach(view => view.Hide());

            OnClickMemoryAsObservable()
                .Subscribe(id =>
                {
                    if (_selectedMemoryIds.Contains(id))
                    {
                        _selectedMemoryIds.Remove(id);
                        _selectedMemoryViews.Find(view => view.MemoryId == id)?.Hide();
                        SetSelected(id, false);
                        return;
                    }

                    if (_selectedMemoryIds.Count >= MaxSelectableCount)
                    {
                        return;
                    }

                    _selectedMemoryIds.Add(id);

                    var view = _selectedMemoryViews.First(view => view.MemoryId.IsEmpty);
                    view.Apply(id, _viewModel.MemoryViewModels.First(m => m.MemoryId == id).MemoryImage);
                    view.Show();

                    SetSelected(id, true);
                })
                .AddTo(this);
        }

        public UniTask ShowAsync(CancellationToken cancellation = default)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            return UniTask.CompletedTask;
        }

        public async UniTask HideAsync(CancellationToken cancellation = default)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            await LMotion.Create(1f, 0f, 1f)
                .BindToAlpha(_canvasGroup)
                .ToUniTask(cancellationToken: cancellation);
        }

        public void ApplyViewModel(ViewModel viewModel)
        {
            _viewModel = viewModel;
            
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
                    .Subscribe(memoryId => _onClickMemory.OnNext(memoryId))
                    .AddTo(memoryView);
                _memoryViews.Add(memoryView);
            }
        }

        public void SetSelected(MemoryId memoryId, bool isSelected)
        {
            var view = _memoryViews.Find(view => view.MemoryId == memoryId);
            view.SetSelected(isSelected);

            if (isSelected)
            {
                _onSelectedMemory.OnNext(memoryId);
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