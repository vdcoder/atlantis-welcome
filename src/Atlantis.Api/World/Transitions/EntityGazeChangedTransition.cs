using Atlantis.Api.Common;

namespace Atlantis.Api.World.Transitions
{
    public sealed record EntityGazeChangedTransition(
        string EntityId,
        Direction GazeDirection)
        : WorldTransition;
}
