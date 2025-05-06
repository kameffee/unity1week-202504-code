using System;
using Unity1week202504.InGame.Photo;

namespace Unity1week202504.InGame.Memories
{
    /// <summary>
    /// 写真の組み合わせを表す構造体
    /// 順番は関係ない
    /// </summary>
    public readonly struct PhotoPair : IEquatable<PhotoPair>
    {
        public PhotoId PhotoId1 { get; }
        public PhotoId PhotoId2 { get; }

        public PhotoPair(PhotoId photoId1, PhotoId photoId2)
        {
            PhotoId1 = photoId1;
            PhotoId2 = photoId2;
        }

        public bool Equals(PhotoPair other)
        {
            // 順番は関係ないので、PhotoId1とPhotoId2の組み合わせが同じなら等しいとみなす
            return PhotoId1.Equals(other.PhotoId1) && PhotoId2.Equals(other.PhotoId2)
                   || PhotoId1.Equals(other.PhotoId2) && PhotoId2.Equals(other.PhotoId1);
        }

        public override bool Equals(object obj)
        {
            return obj is PhotoPair other && Equals(other);
        }

        public override int GetHashCode()
        {
            if (PhotoId1 < PhotoId2)
            {
                return HashCode.Combine(PhotoId1, PhotoId2);
            }

            return HashCode.Combine(PhotoId2, PhotoId1);
        }
    }
}