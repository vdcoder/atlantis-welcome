using Atlantis.Api.Common;

namespace Atlantis.Api.World.Actions
{
    public sealed record SetEntityGazeRequest(
        string ActorId,
        string TargetEntityId,
        Direction GazeDirection)
        : WorldActionRequest(ActorId);
}
