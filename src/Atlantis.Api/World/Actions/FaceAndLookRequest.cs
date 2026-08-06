using Atlantis.Api.Models;

namespace Atlantis.Api.World.Actions
{
    public sealed record FaceAndLookRequest(
        string ActorId,
        string TargetEntityId,
        Direction Direction)
        : WorldActionRequest(ActorId);
}
