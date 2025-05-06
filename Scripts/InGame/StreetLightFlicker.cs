using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unity1week202504.InGame
{
    public class StreetLightFlicker : MonoBehaviour
    {
        float minInterval = 0.2f;
        float maxInterval = 0.7f;

        [SerializeField] GameObject lightOn;
        [SerializeField] GameObject lightOff;

        private void Start()
        {
            FlickerLoop(destroyCancellationToken).Forget();
        }

        private async UniTaskVoid FlickerLoop(CancellationToken cancellationToken)
        {
            var currentToggle = true;
            while (!cancellationToken.IsCancellationRequested)
            {
                lightOn.SetActive(currentToggle);
                lightOff.SetActive(!currentToggle);
                currentToggle = !currentToggle;

                float wait = Random.Range(minInterval, maxInterval);
                await UniTask.Delay(TimeSpan.FromSeconds(wait), cancellationToken: cancellationToken);
            }
        }
    }
}