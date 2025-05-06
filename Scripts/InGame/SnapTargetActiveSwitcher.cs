using Unity1week202504.Data;
using Unity1week202504.InGame.Photo;
using UnityEngine;

namespace Unity1week202504.InGame
{
    public class SnapTargetActiveSwitcher
    {
        private readonly PhotoRepository _photoRepository;
        private readonly PhotoMasterDataSource _photoMasterDataSource;

        public SnapTargetActiveSwitcher(
            PhotoRepository photoRepository,
            PhotoMasterDataSource photoMasterDataSource)
        {
            _photoRepository = photoRepository;
            _photoMasterDataSource = photoMasterDataSource;
        }

        public void SwitchActive(TimeOfDay timeOfDay)
        {
            var timeOfDayObjects = Object.FindObjectsByType<TimeOfDayObject>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            foreach (var timeOfDayObject in timeOfDayObjects)
            {
                // 時間帯に応じてオブジェクトを変更する
                var isShowable = timeOfDayObject.IsShowable(timeOfDay);
                timeOfDayObject.SetVisible(isShowable);
            }

            // 非アクティブなオブジェクトも含めてスナップターゲットを取得
            var snapTargetObjects = Object.FindObjectsByType<SnapTargetObject>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            // 時間帯に応じてスナップターゲットを変更する
            foreach (var snapTargetObject in snapTargetObjects)
            {
                var photoId = snapTargetObject.PhotoId;
                var data = _photoMasterDataSource.Get(snapTargetObject.PhotoId);

                // 槍かつ既に取られてたら表示しない
                var forceHide = _photoRepository.Contains(photoId) && data.IsDisappearableAfterSnap;
                // すべてのスナップターゲットをリセット
                var isShowable = snapTargetObject.IsShowable(timeOfDay);
                snapTargetObject.SetVisible(isShowable && !forceHide);

                // 撮っていなかったら撮影可能
                var snapped = _photoRepository.Contains(photoId);
                snapTargetObject.SetSnappable(!snapped);
            }
        }

        public void SetSnappableAll(bool isSnappable)
        {
            var snapTargetObjects = Object.FindObjectsByType<SnapTargetObject>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            foreach (var snapTargetObject in snapTargetObjects)
            {
                // 撮っていなかったら撮影可能
                var snapped = _photoRepository.Contains(snapTargetObject.PhotoId);
                snapTargetObject.SetSnappable(isSnappable && !snapped);
            }
        }
    }
}