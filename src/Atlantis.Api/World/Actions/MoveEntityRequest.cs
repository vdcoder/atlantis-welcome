using Atlantis.Api.Models;

namespace Atlantis.Api.World.Actions
{
    public sealed record MoveEntityRequest(
        string ActorId,
        string EntityId,
        Position Destination)
        : WorldActionRequest(ActorId);
}
