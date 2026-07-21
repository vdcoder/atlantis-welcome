using Atlantis.Api.Models;

namespace Atlantis.Api.World
{
    public sealed class WorldSnapshot
    {
        public long Revision { get; }
        public Models.World World { get; }

        public WorldSnapshot(long revision, Models.World world)
        {
            Revision = revision;
            World = world;
        }
    }
}
