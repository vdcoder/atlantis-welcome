using Atlantis.Api.Common;
using Atlantis.Api.World.Actions;
using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.Citizens.Control.EmbodiedTools;

public sealed class SetGaze
{
    private readonly WorldActionProcessor
        _actionProcessor;

    public SetGaze(
        WorldActionProcessor actionProcessor)
    {
        _actionProcessor =
            actionProcessor;
    }

    public Task<IReadOnlyList<WorldTransition>>
        SetGazeAsync(
            string entityId,
            Direction gazeDirection)
    {
        return _actionProcessor.ProcessAsync(
            new SetEntityGazeRequest(
                entityId,
                entityId,
                gazeDirection));
    }
}