using Cysharp.Threading.Tasks;
using Unity1week202504.Extensions;

namespace Unity1week202504.InGame.Focus
{
    public class FocusPresenter : Presenter
    {
        private readonly FocusFrameView _focusFrameView;

        public FocusPresenter(FocusFrameView focusFrameView)
        {
            _focusFrameView = focusFrameView;
        }

        public void Show(SnapTargetObject snapTargetObject)
        {
            _focusFrameView.SetTarget(snapTargetObject);
            _focusFrameView.ShowAsync().Forget();
        }

        public void Hide()
        {
            _focusFrameView.Hide();
        }
    }
}