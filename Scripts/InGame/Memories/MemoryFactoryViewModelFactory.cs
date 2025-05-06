using System.Linq;
using Unity1week202504.Data;
using Unity1week202504.InGame.Photo;

namespace Unity1week202504.InGame.Memories
{
    public class MemoryFactoryViewModelFactory
    {
        private readonly PhotoMasterDataSource _photoMasterDataSource;
        private readonly PhotoRepository _photoRepository;
        
        public MemoryFactoryViewModelFactory(
            PhotoMasterDataSource photoMasterDataSource,
            PhotoRepository photoRepository)
        {
            _photoMasterDataSource = photoMasterDataSource;
            _photoRepository = photoRepository;
        }
        
        public MemoryFactoryCanvas.ViewModel Create()
        {
            // 持っている写真を取得
            var viewModels = _photoRepository.All
                .Select(id => _photoMasterDataSource.Get(id))
                .Select(data => new PhotoView.ViewModel(data.Id, data.Sprite, _photoRepository.Contains(data.Id)))
                .OrderByDescending(viewModel => viewModel.HasPhoto)
                .ToArray();
            return new MemoryFactoryCanvas.ViewModel(viewModels);
        }
    }
}