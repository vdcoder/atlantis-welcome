namespace Atlantis.Api.Citizens.Brain.CortexJobs.Tasks;

public sealed record CortexTask
{
    public required Guid Id { get; init; }

    public required Guid CortexJobDefinitionId { get; init; }

    public required string Instructions { get; init; }

    public required decimal Reward { get; init; }

    public required string Currency { get; init; }

    public required string CompletionInboxId { get; init; }

    public required string FailureInboxId { get; init; }

    public required CortexTaskStatus Status { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? AvailableAt { get; init; }

    public DateTimeOffset? UnavailableAt { get; init; }
}