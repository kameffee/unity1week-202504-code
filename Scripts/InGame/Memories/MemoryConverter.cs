using System.Linq;
using NRandom;
using Unity1week202504.Data;
using Unity1week202504.InGame.Photo;
using UnityEngine;

namespace Unity1week202504.InGame.Memories
{
    /// <summary>
    /// 写真から思い出へ変換するクラス
    /// </summary>
    public class MemoryConverter
    {
        private readonly MemoryConditionMasterDataSource _memoryConditionMasterDataSource;

        public MemoryConverter(MemoryConditionMasterDataSource memoryConditionMasterDataSource)
        {
            _memoryConditionMasterDataSource = memoryConditionMasterDataSource;
        }

        public bool TryConvertToMemoryCondition(PhotoId photo1Id, PhotoId photo2Id, out MemoryConditionMasterData result)
        {
            // 必ず条件があることを前提にしている
            var target = new PhotoPair(photo1Id, photo2Id);
            var data = _memoryConditionMasterDataSource.All.FirstOrDefault(data => data.PhotoPair.Equals(target));
            if (data != null)
            {
                result = data;
                return true;
            }

            Debug.LogError($"MemoryConditionMasterData not found: {target.PhotoId1}, {target.PhotoId2} ");
            result = null;
            return false;
        }
    }
}