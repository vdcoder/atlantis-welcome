namespace Atlantis.Api.Citizens.Brain.CortexJobs.Domain;

public sealed record CortexJobScheduleContext
{
    public required DateTimeOffset Now { get; init; }

    public required string WorkerCitizenId { get; init; }

    public string? WorkerLocationEntityId { get; init; }
}