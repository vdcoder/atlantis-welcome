using Atlantis.Api.Citizens.Perception;
using Atlantis.Api.Models;

namespace Atlantis.Api.World.EmbodiedControl;

public sealed record EmbodiedControllerContext
{
    public required Entity Entity { get; init; }

    public required long ObservedWorldRevision { get; init; }

    public required DateTimeOffset ObservedAt { get; init; }

    public required IReadOnlyList<PerceivedObject>
        NearbyObjects
    {
        get;
        init;
    }

    public required IReadOnlyList<PerceivedSensoryOrb>
        SensoryOrbs
    {
        get;
        init;
    }
}