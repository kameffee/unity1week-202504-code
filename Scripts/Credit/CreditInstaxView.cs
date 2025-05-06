using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202504.Credit
{
    public class CreditInstaxView : MonoBehaviour
    {
        [SerializeField]
        private Image _mainImage;

        [SerializeField]
        private Image _blackMask;

        [SerializeField]
        private AnimationCurve _animationCurve = AnimationCurve.EaseInOut(0f, 1f, 0.5f, 1f);

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            _mainImage.enabled = false;
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            _mainImage.enabled = true;
            await LMotion.Create(1f, 0f, 5f)
                .WithEase(_animationCurve)
                .BindToColorA(_blackMask)
                .AddTo(this)
                .ToUniTask(cancellationToken);
        }
    }
}