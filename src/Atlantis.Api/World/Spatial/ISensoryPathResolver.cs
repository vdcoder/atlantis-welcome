using Atlantis.Api.Common;

namespace Atlantis.Api.World.Spatial;

public interface ISensoryPathResolver
{
    bool HasClearPath(
        Position source,
        Position observer,
        World world);
}