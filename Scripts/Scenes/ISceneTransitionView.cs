using Cysharp.Threading.Tasks;

namespace Unity1week202504.Scenes
{
    public interface ISceneTransitionView
    {
        UniTask ShowAsync();

        UniTask HideAsync();
    }
}