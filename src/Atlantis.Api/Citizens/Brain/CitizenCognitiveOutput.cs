using Atlantis.Api.Citizens.Brain.CortexTools;

namespace Atlantis.Api.Citizens.Brain;

public sealed record CitizenCognitiveOutput
{
    public required IReadOnlyList<Prediction>
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