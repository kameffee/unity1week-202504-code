using R3;
using Unity1week202504.Extensions;
using VContainer.Unity;

namespace Unity1week202504.InGame.Cameras
{
    public class CameraMoveControlPresenter : Presenter, IStartable
    {
        private readonly CameraMoveControlView _view;
        private readonly InGameCamera _camera;

        public CameraMoveControlPresenter(
            CameraMoveControlView view,
            InGameCamera camera)
        {
            _view = view;
            _camera = camera;
        }

        public void Start()
        {
            _view.OnLeftAsObservable()
                .Subscribe(_ => _camera.MoveToLeft())
                .AddTo(this);

            _view.OnRightAsObservable()
                .Subscribe(_ => _camera.MoveToRight())
                .AddTo(this);

            _camera.CurrentTrackingPointIndex
                .Subscribe(_ =>
                {
                    _view.SetVisibleLeft(_camera.IsLeftMovable());
                    _view.SetVisibleRight(_camera.IsRightMovable());
                })
                .AddTo(this);
        }
    }
}