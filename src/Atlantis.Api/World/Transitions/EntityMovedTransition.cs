using Atlantis.Api.Models;

namespace Atlantis.Api.World.Transitions
{
    public sealed record EntityMovedTransition(
        string EntityId,
        Position From,
        Position To)
        : WorldTransition;
}
