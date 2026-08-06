namespace Atlantis.Api.Citizens.Brain.CortexJobs.Completion;

public sealed record CompleteCortexTaskRequest
{
    public required string WorkerCitizenId { get; init; }

    public required Guid AssignmentId { get; init; }

    public required string ResultSerialized { get; init; }
}