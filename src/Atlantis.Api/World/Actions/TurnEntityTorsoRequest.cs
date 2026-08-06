using Atlantis.Api.Models;

namespace Atlantis.Api.World.Actions
{
    public sealed record TurnEntityTorsoRequest(
        string ActorId,
        string TargetEntityId,
        Direction TorsoFront)
        : WorldActionRequest(ActorId);
}
