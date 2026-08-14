using Atlantis.Api.Common;
using Atlantis.Api.World.Actions;
using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.Citizens.Control.EmbodiedTools;

public sealed class TurnTorso
{
    private readonly WorldActionProcessor
        _actionProcessor;

    public TurnTorso(
        WorldActionProcessor actionProcessor)
    {
        _actionProcessor =
            actionProcessor;
    }

    public Task<IReadOnlyList<WorldTransition>>
        TurnTorsoAsync(
            string entityId,
            Direction torsoFront)
    {
        return _actionProcessor.ProcessAsync(
            new TurnEntityTorsoRequest(
                entityId,
                entityId,
                torsoFront));
    }
}