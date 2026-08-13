using Atlantis.Api.Common;

namespace Atlantis.Api.World.Actions
{
    public sealed record TurnEntityTorsoRequest(
        string ActorId,
        string TargetEntityId,
        Direction TorsoFront)
        : WorldActionRequest(ActorId);
}
