namespace Atlantis.Api.Citizens.Brain.CortexJobs.Simulation;

public sealed record SimulateCitizenPassTask
{
    public required Guid CortexTaskId { get; init; }

    public required string ExternalCitizenId { get; init; }

    public required string ExternalWorldId { get; init; }

    public required string ExternalRequestId { get; init; }

    public required string ExternalScenarioId { get; init; }

    public required DateTimeOffset ReceivedAt { get; init; }
}