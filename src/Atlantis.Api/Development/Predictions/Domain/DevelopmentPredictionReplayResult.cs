using Atlantis.Api.Development.Predictions.Contracts;

namespace Atlantis.Api.Development.Predictions.Domain;

public sealed record DevelopmentPredictionReplayResult
{
    public required DevelopmentPredictionReplayStatus Status
    {
        get;
        init;
    }

    public Guid? RecordedPredictionId
    {
        get;
        init;
    }

    public RecordedCitizenBreathDto? RecordedBreath
    {
        get;
        init;
    }

    public string? WorkerCitizenId
    {
        get;
        init;
    }

    public static DevelopmentPredictionReplayResult Missing()
    {
        return new DevelopmentPredictionReplayResult
        {
            Status =
                DevelopmentPredictionReplayStatus.Missing
        };
    }

    public static DevelopmentPredictionReplayResult Ready(
        Guid recordedPredictionId,
        RecordedCitizenBreathDto recordedBreath,
        string workerCitizenId)
    {
        ArgumentNullException.ThrowIfNull(
            recordedBreath);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            workerCitizenId);

        return new DevelopmentPredictionReplayResult
        {
            Status =
                DevelopmentPredictionReplayStatus.Ready,

            RecordedPredictionId =
                recordedPredictionId,

            RecordedBreath =
                recordedBreath,

            WorkerCitizenId =
                workerCitizenId
        };
    }
}