using Unity1week202504.InGame.Memories;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202504.Ending
{
    public class SelectedMemoryView : MonoBehaviour
    {
        public MemoryId MemoryId { get; private set; } = MemoryId.Empty;
        
        [SerializeField]
        private Image _memoryImage;

        public void Apply(MemoryId memoryId, Sprite memoryImage)
        {
            _memoryImage.sprite = memoryImage;
            MemoryId = memoryId;
            gameObject.SetActive(true);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            MemoryId = MemoryId.Empty;
            gameObject.SetActive(false);
        }
    }
}