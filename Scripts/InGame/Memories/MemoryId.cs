using UnitGenerator;

namespace Unity1week202504.InGame.Memories
{
    [UnitOf(typeof(int))]
    public readonly partial struct MemoryId
    {
        public static MemoryId Empty => new(-1);
        public bool IsEmpty => value == -1;
    }
}