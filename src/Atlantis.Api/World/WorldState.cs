namespace Atlantis.Api.World;

public sealed class WorldState
{
    public World World
    {
        get;
        private set;
    } = null!;

    public long Revision
    {
        get;
        private set;
    }

    public void Initialize(
        World world,
        long revision = 0)
    {
        World = world;
        Revision = revision;
    }

    public void AdvanceRevision()
    {
        Revision++;
    }

    public void SetRevision(
        long revision)
    {
        Revision = revision;
    }
}