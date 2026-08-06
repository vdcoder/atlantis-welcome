namespace Atlantis.Api.Citizens.Brain.CortexJobs.Simulation;

public sealed record SimulateCitizenPassResult
{
    public required Guid CortexTaskResultId { get; init; }

    public required string ExternalCitizenId { get; init; }

    public required string ExternalWorldId { get; init; }

    public required string ExternalRequestId { get; init; }

    public required string ExternalScenarioId { get; init; }

    public required string PassPredictionListSerialized
    {
        get;
        init;
    }

    public required DateTimeOffset CompletedAt { get; init; }
}