using Unity1week202504.InGame.Photo;

namespace Unity1week202504
{
    public class GameResetter
    {
        private readonly PhotoRepository _photoRepository;

        public GameResetter(PhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        /// <summary>
        /// 周回時に通る処理
        /// </summary>
        public void Reset()
        {
            // 撮った写真のみ初期化
            _photoRepository.Clear();
        }
    }
}