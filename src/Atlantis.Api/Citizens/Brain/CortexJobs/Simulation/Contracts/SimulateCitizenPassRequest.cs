namespace Atlantis.Api.Citizens.Brain.CortexJobs.Simulation.Contracts;

public sealed record SimulateCitizenPassRequest
{
    public required string ExternalCitizenId { get; init; }

    public required string ExternalWorldId { get; init; }

    public required string ExternalRequestId { get; init; }

    public required string ExternalScenarioId { get; init; }

    public required string
        PassContextAndCurrentPositionSerialized
    {
        get;
        init;
    }

    public required DateTimeOffset ReceivedAt { get; init; }
}