using Atlantis.Api.World.Actions;

namespace Atlantis.Api.World.Actions
{
    public sealed record SayRequest(
        string ActorId,
        string EntityId,
        string Text)
        : WorldActionRequest(ActorId);
}
