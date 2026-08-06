namespace Atlantis.Api.Citizens.Brain.CortexJobs.Tasks;

public sealed record CortexTaskResult
{
    public required Guid Id { get; init; }

    public required Guid CortexTaskAssignmentId { get; init; }

    public required string ResultType { get; init; }

    public required string ResultSerialized { get; init; }

    public required DateTimeOffset CompletedAt { get; init; }
}