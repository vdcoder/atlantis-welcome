using Atlantis.Api.Citizens.Brain.CortexTools;
using Atlantis.Api.World.EmbodiedControl;

namespace Atlantis.Api.Citizens.Control;

public sealed record CitizenBreath
{
    public required PredictionBatch
        EmbodiedPredictions
    {
        get;
        init;
    }

    public required IReadOnlyList<CortexToolCall>
        CortexToolCalls
    {
        get;
        init;
    }
}