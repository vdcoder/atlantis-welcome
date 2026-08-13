using Atlantis.Api.Common;

namespace Atlantis.Api.World.Spatial;

public sealed class AlwaysClearSensoryPathResolver
    : ISensoryPathResolver
{
    public bool HasClearPath(
        Position source,
        Position observer,
        World world)
    {
        return true;
    }
}
