using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Extensions;
using VContainer.Unity;

namespace Unity1week202504.Credit
{
    public class CreditPresenter : Presenter, IInitializable
    {
        private readonly CreditView _creditView;

        public CreditPresenter(CreditView creditView)
        {
            _creditView = creditView;
        }

        public void Initialize()
        {
            _creditView.OnClickCloseAsObservable()
                .SubscribeAwait(async (_, ct) => await _creditView.HideAsync(ct), AwaitOperation.Drop)
                .AddTo(this);
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            await _creditView.ShowAsync(cancellationToken);
        }
    }
}