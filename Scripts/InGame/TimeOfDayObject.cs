using UnityEngine;

namespace Unity1week202504.InGame
{
    public class TimeOfDayObject : MonoBehaviour
    {
        public TimeOfDay TimeOfDay => _timeOfDay;

        [SerializeField]
        private TimeOfDay _timeOfDay;

        public bool IsShowable(TimeOfDay timeOfDay)
        {
            return _timeOfDay == timeOfDay;
        }

        public void SetVisible(bool isShowable)
        {
            gameObject.SetActive(isShowable);
        }
    }
}