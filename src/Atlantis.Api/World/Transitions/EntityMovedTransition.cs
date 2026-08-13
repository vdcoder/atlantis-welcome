using Atlantis.Api.Common;

namespace Atlantis.Api.World.Transitions
{
    public sealed record EntityMovedTransition(
        string EntityId,
        Position From,
        Position To,
        DateTimeOffset ChangedAt)
        : WorldTransition;
}
