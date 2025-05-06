using System;
using Alchemy.Inspector;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202504.InGame.Memories
{
    public class MemoryView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _root;

        [SerializeField]
        private Image _memoryImage;

        [SerializeField]
        private GameObject _unlockImageObject;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private Color _normalColor = new(0.6f, 0.6f, 0.6f, 1f);

        [SerializeField]
        private Color _selectedColor =  Color.white;

        [ReadOnly]
        [ShowInInspector]
        public MemoryId MemoryId { get; private set; }

        private void Awake()
        {
            SetSelected(false);
        }

        public void Apply(ViewModel viewModel)
        {
            MemoryId = viewModel.MemoryId;
            _memoryImage.sprite = viewModel.IsUnlocked ? viewModel.MemoryImage : null;
            _memoryImage.enabled = viewModel.IsUnlocked;
            _unlockImageObject.SetActive(viewModel.IsLocked);
            _button.interactable = viewModel.IsUnlocked;
            _root.SetActive(true);
        }

        public Observable<MemoryId> OnClickAsObservable() => _button.OnClickAsObservable().Select(_ => MemoryId);

        public void SetSelected(bool isSelected)
        {
            // 選択状態は白、非選択状態は薄暗い感じ
            _memoryImage.color = isSelected ? _selectedColor : _normalColor;
        }

        public class ViewModel
        {
            public MemoryId MemoryId { get; }
            public Sprite MemoryImage { get; }
            public bool IsUnlocked { get; }
            public bool IsLocked => !IsUnlocked;

            public ViewModel(MemoryId id, Sprite memoryImage, bool isUnlocked)
            {
                MemoryId = id;
                MemoryImage = memoryImage;
                IsUnlocked = isUnlocked;
            }
        }
    }
}