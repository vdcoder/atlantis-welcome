using Atlantis.Api.Models;

namespace Atlantis.Api.World
{
    public sealed class WorldState
    {
        public Models.World World { get; private set; } = null!;

        public long Revision { get; private set; }

        public void Initialize(
            Models.World world,
            long revision = 0)
        {
            World = world;
            Revision = revision;
        }

        public void AdvanceRevision()
        {
            Revision++;
        }

        public void SetRevision(long revision)
        {
            Revision = revision;
        }
    }
}
