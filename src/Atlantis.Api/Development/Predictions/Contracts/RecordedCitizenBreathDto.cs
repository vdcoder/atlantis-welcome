namespace Atlantis.Api.Development.Predictions.Contracts;

public sealed record RecordedCitizenBreathDto
{
    public required IReadOnlyList<RecordedPredictionDto>
        EmbodiedPredictions
    {
        get;
        init;
    }

    public required IReadOnlyList<RecordedCortexToolCallDto>
        CortexToolCalls
    {
        get;
        init;
    }
}