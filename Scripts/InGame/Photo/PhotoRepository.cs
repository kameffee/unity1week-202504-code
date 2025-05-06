using System.Collections.Generic;

namespace Unity1week202504.InGame.Photo
{
    public class PhotoRepository
    {
        public IEnumerable<PhotoId> All => _photos;

        private readonly List<PhotoId> _photos = new();

        public void Add(PhotoId photoId)
        {
            _photos.Add(photoId);
        }

        public bool Contains(PhotoId id) => _photos.Contains(id);

        public void Clear() => _photos.Clear();
    }
}