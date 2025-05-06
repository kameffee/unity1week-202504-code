using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using Unity1week202504.Audio.Player;
using UnityEngine;
using VContainer;

namespace Unity1week202504.InGame.Focus
{
    public class FocusFrameView : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        private AudioPlayer _audioPlayer;
        private CancellationTokenSource _cancellationTokenSource = new();

        [Inject]
        public void Construct(AudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
        }

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void SetTarget(SnapTargetObject snapTargetObject)
        {
            transform.position = snapTargetObject.WorldPosition;
        }

        public async UniTask ShowAsync()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            _audioPlayer.PlaySe("TakePhotoPhase/Focus");

            gameObject.SetActive(true);
            var sequence = LSequence.Create();
            sequence
                .Append(LMotion.Create(1f, 0f, 0.02f).BindToColorA(_spriteRenderer))
                .AppendInterval(0.05f)
                .Append(LMotion.Create(0f, 1f, 0.02f).BindToColorA(_spriteRenderer))
                .AppendInterval(0.05f)
                .Append(LMotion.Create(1f, 0f, 0.02f).BindToColorA(_spriteRenderer))
                .AppendInterval(0.05f)
                .Append(LMotion.Create(0f, 1f, 0.02f).BindToColorA(_spriteRenderer));

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                _cancellationTokenSource.Token,
                destroyCancellationToken);

            await sequence.Run().ToUniTask(cancellationTokenSource.Token);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}