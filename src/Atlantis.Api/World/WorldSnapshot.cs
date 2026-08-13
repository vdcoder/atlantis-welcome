namespace Atlantis.Api.World
{
    public sealed class WorldSnapshot
    {
        public long Revision { get; }
        public World World { get; }

        public WorldSnapshot(long revision, World world)
        {
            Revision = revision;
            World = world;
        }
    }
}
