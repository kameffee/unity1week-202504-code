using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202504.Audio.Player;
using Unity1week202504.Data;
using Unity1week202504.Data.Messages;
using Unity1week202504.Message;
using UnityEngine;

namespace Unity1week202504.InGame.Memories
{
    public class MemoryGeneratePhase
    {
        private readonly MemoryConverter _memoryConverter;
        private readonly MemoryRepository _memoryRepository;
        private readonly MemoryAfterPerformer _memoryAfterPerformer;
        private readonly MemoryFactoryCanvas _memoryFactoryCanvas;
        private readonly MemoryFactoryViewModelFactory _memoryFactoryViewModelFactory;
        private readonly MemoryGenerateHistoryRepository _memoryGenerateHistoryRepository;
        private readonly MessagePlayer _messagePlayer;
        private readonly MessageBlockMasterDataSource _messageBlockMasterDataSource;
        private readonly AudioPlayer _audioPlayer;
        private readonly InGameSettings _inGameSettings;

        public MemoryGeneratePhase(
            MemoryConverter memoryConverter,
            MemoryRepository memoryRepository,
            MemoryAfterPerformer memoryAfterPerformer,
            MemoryFactoryCanvas memoryFactoryCanvas,
            MemoryFactoryViewModelFactory memoryFactoryViewModelFactory,
            MemoryGenerateHistoryRepository memoryGenerateHistoryRepository,
            MessagePlayer messagePlayer,
            MessageBlockMasterDataSource messageBlockMasterDataSource,
            AudioPlayer audioPlayer,
            InGameSettings inGameSettings)
        {
            _memoryConverter = memoryConverter;
            _memoryRepository = memoryRepository;
            _memoryAfterPerformer = memoryAfterPerformer;
            _memoryFactoryCanvas = memoryFactoryCanvas;
            _memoryFactoryViewModelFactory = memoryFactoryViewModelFactory;
            _memoryGenerateHistoryRepository = memoryGenerateHistoryRepository;
            _messagePlayer = messagePlayer;
            _messageBlockMasterDataSource = messageBlockMasterDataSource;
            _audioPlayer = audioPlayer;
            _inGameSettings = inGameSettings;
        }

        public async UniTask Execute(TimeOfDay timeOfDay, CancellationToken cancellationToken)
        {
            Debug.Log("MemoryPhase.Execute");

            _memoryFactoryCanvas.SetInteractable(false);

            // 表示開始
            var viewModel = _memoryFactoryViewModelFactory.Create();
            _memoryFactoryCanvas.ApplyViewModel(viewModel);

            await _audioPlayer.StopBgm(0.5f);
            await _memoryFactoryCanvas.ShowAsync(cancellationToken);

            _audioPlayer.PlayBgm("MemoryGeneratePhase/SelectPhoto");

            // チュートリアル
            if (timeOfDay == TimeOfDay.Morning)
            {
                var messageBlock = _messageBlockMasterDataSource.Get("MemoryGenerateTutorial").MessageBlock;
                await _messagePlayer.PlayAsync(messageBlock, MessageMode.Step, cancellationToken);
            }

            var generateCount = _inGameSettings.GetMemoryGenerateCount(timeOfDay);
            // 2回行う
            for (int i = 0; i < generateCount; i++)
            {
                // 初期化
                _memoryFactoryCanvas.Reset();

                _memoryFactoryCanvas.SetInteractable(true);

                // 写真一覧を表示. 2枚選ぶ
                var photoIds = await _memoryFactoryCanvas.OnSubmitAsObservable()
                    .FirstAsync(cancellationToken);

                // ボタン類を無効化
                _memoryFactoryCanvas.SetInteractable(false);

                // 2枚の写真から思い出へ変換
                var valid = _memoryConverter.TryConvertToMemoryCondition(photoIds.photoId1, photoIds.photoId2,
                    out var conditionData);

                // 試した写真の組み合わせを保存
                _memoryGenerateHistoryRepository.Add(new PhotoPair(photoIds.photoId1, photoIds.photoId2));

                if (!valid) continue;

                Debug.Log($"{photoIds.Item1} + {photoIds.Item2} -> MemoryId: {conditionData.OutputMemoryId}",
                    conditionData);

                if (conditionData.OutputMemoryId.IsEmpty)
                {
                    // 失敗時はメッセージのみ再生
                    var messageBlock = conditionData.GeneratedComment;
                    await _messagePlayer.PlayAsync(messageBlock, MessageMode.Step, cancellationToken);
                    continue;
                }

                // 思い出が出力されたら一覧に登録
                var memoryId = conditionData.OutputMemoryId;
                _memoryRepository.Add(memoryId);

                // 思い出演出
                await _memoryAfterPerformer.PlayAsync(conditionData.Id, cancellationToken);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);

            if (TryGetAfterMessage(timeOfDay, out var afterMessage))
            {
                await _messagePlayer.PlayAsync(afterMessage, MessageMode.Step, cancellationToken);
            }

            // 思い出の演出が終わったら、思い出を非表示にする
            await _memoryFactoryCanvas.HideAsync(cancellationToken);
        }

        private bool TryGetAfterMessage(TimeOfDay timeOfDay, out MessageBlock messageBlock)
        {
            var key = GetKey(timeOfDay);
            if (!string.IsNullOrEmpty(key))
            {
                messageBlock = _messageBlockMasterDataSource.Get(key).MessageBlock;
                return true;
            }

            messageBlock = null;
            return false;

            static string GetKey(TimeOfDay timeOfDay)
            {
                return timeOfDay switch
                {
                    TimeOfDay.Morning => "MemoryGenerateAfter/Morning",
                    TimeOfDay.Afternoon => "MemoryGenerateAfter/Afternoon",
                    TimeOfDay.Night => string.Empty, // 夜はなし
                    _ => throw new ArgumentOutOfRangeException(nameof(timeOfDay), timeOfDay, null)
                };
            }
        }
    }
}