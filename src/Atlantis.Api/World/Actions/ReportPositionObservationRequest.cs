using Atlantis.Api.Common;

namespace Atlantis.Api.World.Actions;

public sealed record ReportPositionObservationRequest(
    string ActorId,
    string ObservedEntityId,
    Position Position)
    : WorldActionRequest(ActorId);
