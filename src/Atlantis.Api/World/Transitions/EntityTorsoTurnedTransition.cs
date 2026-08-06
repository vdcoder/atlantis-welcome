using Atlantis.Api.Models;

namespace Atlantis.Api.World.Transitions
{
    public sealed record EntityTorsoTurnedTransition(
        string EntityId,
        Direction TorsoFront)
        : WorldTransition;
}
