using System.Threading;

namespace Unity1week202504.Title
{
    using UnityEngine;
    using Cysharp.Threading.Tasks;

    [DisallowMultipleComponent]
    public class TitleBackgroundView : MonoBehaviour
    {
        [SerializeField] Transform targetTransform;
        
        [Header("Shake Settings")] [SerializeField]
        float amplitude = 0.1f;

        [SerializeField] float speed = 1f;

        Vector3 _originalPosition;

        void Awake()
        {
            _originalPosition = targetTransform.localPosition;
        }

        void Start()
        {
            JitterLoop(destroyCancellationToken).Forget();
        }

        async UniTaskVoid JitterLoop(CancellationToken cancellationToken)
        {
            Vector3 currentOffset = Vector3.zero;

            while (!cancellationToken.IsCancellationRequested)
            {
                Vector2 rnd = Random.insideUnitCircle * amplitude;
                Vector3 nextOffset = new Vector3(rnd.x, rnd.y, 0f);
                float elapsed = 0f;
                float duration = 1f / Mathf.Max(speed, 0.0001f);
                Vector3 startOffset = currentOffset;

                while (elapsed < duration && !cancellationToken.IsCancellationRequested)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);
                    currentOffset = Vector3.Lerp(startOffset, nextOffset, t);
                    targetTransform.localPosition = _originalPosition + currentOffset;
                    await UniTask.Yield(PlayerLoopTiming.Update);
                }

                // Prepare for next cycle
                currentOffset = nextOffset;
            }
        }

    }
}