using Atlantis.Api.Models;

namespace Atlantis.Api.World.Transitions
{
    public sealed record EntityGazeChangedTransition(
        string EntityId,
        Direction GazeDirection)
        : WorldTransition;
}
