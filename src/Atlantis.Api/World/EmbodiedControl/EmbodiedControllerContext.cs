using Atlantis.Api.Citizens.Perception;
using Atlantis.Api.World.Entities;

namespace Atlantis.Api.World.EmbodiedControl;

public sealed record EmbodiedControllerContext
{
    public required Entity Entity { get; init; }

    public required long ObservedWorldRevision { get; init; }

    public required DateTimeOffset ObservedAt { get; init; }

    public required IReadOnlyList<TransparentEntity>
        NearbyEntities
    {
        get;
        init;
    }

    public required IReadOnlyList<TransparentAuditoryEvent>
        AuditoryEvents
    {
        get;
        init;
    }
}