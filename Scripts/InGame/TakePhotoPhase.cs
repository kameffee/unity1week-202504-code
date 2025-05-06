using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Audio.Player;
using Unity1week202504.Data;
using Unity1week202504.Data.Messages;
using Unity1week202504.Extensions;
using Unity1week202504.InGame.Focus;
using Unity1week202504.InGame.Photo;
using Unity1week202504.Message;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer.Unity;

namespace Unity1week202504.InGame
{
    public class TakePhotoPhase : Presenter, ITickable
    {
        private const int MaxSnapCount = 3;

        private readonly PhotoRepository _photoRepository;
        private readonly MessagePlayer _messagePlayer;
        private readonly MessageBlockMasterDataSource _messageBlockMasterDataSource;
        private readonly SnapTargetActiveSwitcher _snapTargetActiveSwitcher;
        private readonly AudioPlayer _audioPlayer;
        private readonly PhotoMasterDataSource _photoMasterDataSource;
        private readonly FocusPresenter _focusPresenter;

        private readonly Subject<PhotoId> _onSelectSnapTarget = new();
        private readonly ReactiveProperty<SnapTargetObject> _currentFocusSnapTargetObject = new();
        private bool _isActive = true;

        public TakePhotoPhase(
            PhotoRepository photoRepository,
            MessagePlayer messagePlayer,
            MessageBlockMasterDataSource messageBlockMasterDataSource,
            SnapTargetActiveSwitcher snapTargetActiveSwitcher,
            AudioPlayer audioPlayer,
            PhotoMasterDataSource photoMasterDataSource,
            FocusPresenter focusPresenter)
        {
            _photoRepository = photoRepository;
            _messagePlayer = messagePlayer;
            _messageBlockMasterDataSource = messageBlockMasterDataSource;
            _snapTargetActiveSwitcher = snapTargetActiveSwitcher;
            _audioPlayer = audioPlayer;
            _photoMasterDataSource = photoMasterDataSource;
            _focusPresenter = focusPresenter;

            _currentFocusSnapTargetObject
                .Subscribe(o =>
                {
                    if (o != null)
                    {
                        Debug.Log($"currentFocus: {o.name}", o.gameObject);
                    }
                    else
                    {
                        Debug.Log("currentFocus: None");
                    }
                })
                .AddTo(this);
        }

        public async UniTask Execute(TimeOfDay timeOfDay, CancellationToken cancellationToken)
        {
            PlayBgm(timeOfDay);

            // 必要枚数分スナップする
            for (var i = 0; i < MaxSnapCount; i++)
            {
                _isActive = true;

                Debug.Log($"フェーズ{timeOfDay.ToString()} : {i + 1}枚目のスナップタイム");
                var photoId = await _onSelectSnapTarget.FirstAsync(cancellationToken: cancellationToken);

                // 撮った写真を保存
                _photoRepository.Add(photoId);

                _isActive = false;
            }

            // これ以上撮れないよう無効にしておく
            _snapTargetActiveSwitcher.SetSnappableAll(false);

            // 撮影フェーズ終了
            await _messagePlayer.WaitEndAsync(cancellationToken);

            var messageBlock = GetMessage(timeOfDay);
            await _messagePlayer.PlayAsync(messageBlock, MessageMode.Step, cancellationToken);
        }

        private void PlayBgm(TimeOfDay timeOfDay)
        {
            var id = GetId();
            _audioPlayer.PlayBgm(id);
            return;

            string GetId()
            {
                return timeOfDay switch
                {
                    TimeOfDay.Morning => "TakePhotoPhase/Morning",
                    TimeOfDay.Afternoon => "TakePhotoPhase/Afternoon",
                    TimeOfDay.Night => "TakePhotoPhase/Night",
                    _ => throw new ArgumentOutOfRangeException(nameof(timeOfDay), timeOfDay, null)
                };
            }
        }

        private MessageBlock GetMessage(TimeOfDay timeOfDay)
        {
            var key = GetKey();
            return _messageBlockMasterDataSource.Get(key).MessageBlock;

            string GetKey()
            {
                return timeOfDay switch
                {
                    TimeOfDay.Morning => "BeforeMemoryFactory/Morning",
                    TimeOfDay.Afternoon => "BeforeMemoryFactory/Afternoon",
                    TimeOfDay.Night => "BeforeMemoryFactory/Night",
                    _ => throw new ArgumentOutOfRangeException(nameof(timeOfDay), timeOfDay, null)
                };
            }
        }

        public void Tick()
        {
            if (!_isActive) return;

            var mousePosition = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                if (TryGetSnapTargetObject(mousePosition, out var snapTargetObject))
                {
                    if (snapTargetObject.Snappable)
                    {
                        if (_currentFocusSnapTargetObject.Value == null ||
                            _currentFocusSnapTargetObject.Value != snapTargetObject)
                        {
                            _currentFocusSnapTargetObject.Value = snapTargetObject;
                            _focusPresenter.Show(snapTargetObject);
                            var masterData = _photoMasterDataSource.Get(snapTargetObject.PhotoId);

                            // フォーカス時のコメント
                            _messagePlayer.PlayAsync(masterData.FocusSpeech, MessageMode.Auto).Forget();
                        }
                        else if (_currentFocusSnapTargetObject.Value == snapTargetObject)
                        {
                            // 撮影
                            snapTargetObject.Snap();

                            // フォーカスから外す
                            _currentFocusSnapTargetObject.Value = null;
                            _focusPresenter.Hide();

                            // 一度スナップしたらスナップできないようにする
                            snapTargetObject.SetSnappable(false);

                            var photoId = snapTargetObject.PhotoId;
                            _onSelectSnapTarget.OnNext(photoId);
                        }
                    }
                }
                else
                {
                    _currentFocusSnapTargetObject.Value = null;
                    _focusPresenter.Hide();
                }
            }
        }

        private bool TryGetSnapTargetObject(Vector2 mousePosition, out SnapTargetObject snapTargetObject)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
            var hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider != null)
            {
                snapTargetObject = hit.collider.gameObject.GetComponent<SnapTargetObject>();
                return snapTargetObject != null;
            }

            snapTargetObject = null;
            return false;
        }
    }
}