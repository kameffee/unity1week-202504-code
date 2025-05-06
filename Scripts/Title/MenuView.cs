using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202504.Title
{
    public class MenuView : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private Button _licenseButton;

        [SerializeField]
        Button _creditButton;

        public Observable<Unit> OnClickStartAsObservable() => _startButton.OnClickAsObservable();
        public Observable<Unit> OnClickLicenseAsObservable() => _licenseButton.OnClickAsObservable();
        public Observable<Unit> OnClickCreditAsObservable() => _creditButton.OnClickAsObservable();
    }
}