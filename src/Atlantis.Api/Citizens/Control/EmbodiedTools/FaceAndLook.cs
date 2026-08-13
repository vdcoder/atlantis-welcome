using Atlantis.Api.Common;
using Atlantis.Api.World.Actions;
using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.Citizens.Control.EmbodiedTools;

public sealed class FaceAndLook
{
    private readonly WorldActionProcessor
        _actionProcessor;

    public FaceAndLook(
        WorldActionProcessor actionProcessor)
    {
        _actionProcessor =
            actionProcessor;
    }

    public Task<IReadOnlyList<WorldTransition>>
        FaceAndLookAsync(
            string entityId,
            Direction direction)
    {
        return _actionProcessor.ProcessAsync(
            new FaceAndLookRequest(
                entityId,
                entityId,
                direction));
    }
}