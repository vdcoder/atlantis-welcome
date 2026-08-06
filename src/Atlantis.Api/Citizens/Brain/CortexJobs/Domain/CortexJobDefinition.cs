namespace Atlantis.Api.Citizens.Brain.CortexJobs.Domain;

public sealed record CortexJobDefinition
{
    public required Guid Id { get; init; }

    public required string EmployerAccountId { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string Qualifications { get; init; }

    public required CortexJobSchedule Schedule { get; init; }

    public required string CompletionInboxId { get; init; }

    public required string FailureInboxId { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? DeactivatedAt { get; init; }

    public bool IsActiveAt(DateTimeOffset time) =>
        DeactivatedAt is null ||
        time < DeactivatedAt;
}