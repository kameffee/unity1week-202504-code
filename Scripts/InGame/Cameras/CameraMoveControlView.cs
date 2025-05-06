using System;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202504.InGame.Cameras
{
    public class CameraMoveControlView : MonoBehaviour
    {
        [SerializeField]
        private Image _left;

        [SerializeField]
        private Image _right;

        public Observable<Unit> OnLeftAsObservable() => _left.OnPointerEnterAsObservable().AsUnitObservable();
        public Observable<Unit> OnRightAsObservable() => _right.OnPointerEnterAsObservable().AsUnitObservable();

        public void SetVisibleLeft(bool isVisible)
        {
            _left.gameObject.SetActive(isVisible);
        }

        public void SetVisibleRight(bool isVisible)
        {
            _right.gameObject.SetActive(isVisible);
        }
    }
}