using Unity1week202504.InGame.Photo;
using UnityEngine;

namespace Unity1week202504.Data
{
    [CreateAssetMenu(fileName = "PhotoMasterDataSource", menuName = "MasterData/Photo/DataSource", order = 0)]
    public class PhotoMasterDataSource : ScriptableObject
    {
        [SerializeField]
        private PhotoMasterData[] _photoMasterData;

        public PhotoMasterData[] All => _photoMasterData;

        public PhotoMasterData Get(PhotoId id)
        {
            foreach (var data in _photoMasterData)
            {
                if (data.Id == id)
                {
                    return data;
                }
            }

            Debug.LogError($"PhotoMasterData not found: {id}");
            return null;
        }
    }
}