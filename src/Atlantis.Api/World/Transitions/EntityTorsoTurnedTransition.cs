using Atlantis.Api.Common;

namespace Atlantis.Api.World.Transitions
{
    public sealed record EntityTorsoTurnedTransition(
        string EntityId,
        Direction TorsoFront)
        : WorldTransition;
}
