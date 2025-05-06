using System.Collections.Generic;
using Alchemy.Inspector;
using R3;
using UnityEngine;

namespace Unity1week202504.InGame.Cameras
{
    public class InGameCamera : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> _trackingPoints;

        [SerializeField]
        private int _defaultTrackingPointIndex = 1;

        [SerializeField]
        private Transform _controlTrackingPoint;

        [SerializeField]
        private SerializableReactiveProperty<int> _currentTrackingPointIndex = new(0);

        public ReadOnlyReactiveProperty<int> CurrentTrackingPointIndex => _currentTrackingPointIndex;

        private void Awake()
        {
            _currentTrackingPointIndex.Value = _defaultTrackingPointIndex;

            _currentTrackingPointIndex
                .Subscribe(x => MoveTo(Clamp(x)))
                .AddTo(this);
        }

        [HorizontalGroup("Change Tracking Points")]
        [Button]
        public void MoveToLeft()
        {
            var nextIndex = _currentTrackingPointIndex.Value - 1;

            MoveTo(Clamp(nextIndex));
        }

        [HorizontalGroup("Change Tracking Points")]
        [Button]
        public void MoveToRight()
        {
            var nextIndex = _currentTrackingPointIndex.Value + 1;

            MoveTo(Clamp(nextIndex));
        }

        public bool IsLeftMovable()
        {
            return _currentTrackingPointIndex.Value > 0;
        }

        public bool IsRightMovable()
        {
            return _currentTrackingPointIndex.Value < _trackingPoints.Count - 1;
        }


        private int Clamp(int value)
        {
            return Mathf.Clamp(value, 0, _trackingPoints.Count - 1);
        }

        private void MoveTo(int index)
        {
            if (index < 0 || index >= _trackingPoints.Count)
            {
                Debug.LogError($"Invalid tracking point index: {index}");
                return;
            }

            var toPoint = _trackingPoints[index];
            _controlTrackingPoint.position = toPoint.position;

            _currentTrackingPointIndex.Value = index;
        }

        public void SetDefaultPosition()
        {
            _currentTrackingPointIndex.Value = _defaultTrackingPointIndex;
        }
    }
}