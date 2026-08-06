using Atlantis.Api.Development.Predictions.Contracts;

namespace Atlantis.Api.Development.Predictions.Remote;

public sealed record TrueWorldPredictionResponseDto
{
    public required string WorkerCitizenId
    {
        get;
        init;
    }

    public required DateTimeOffset RecordedAt
    {
        get;
        init;
    }

    public required RecordedCitizenBreathDto
        RecordedBreath
    {
        get;
        init;
    }
}