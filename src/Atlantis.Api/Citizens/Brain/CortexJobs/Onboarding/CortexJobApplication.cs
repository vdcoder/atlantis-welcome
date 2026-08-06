namespace Atlantis.Api.Citizens.Brain.CortexJobs.Onboarding;

public sealed record CortexJobApplication
{
    public required Guid Id { get; init; }

    public required Guid CortexJobDefinitionId { get; init; }

    public required string WorkerCitizenId { get; init; }

    public required string DepositAccountId { get; init; }

    public required CortexJobApplicationStatus Status { get; init; }

    public required DateTimeOffset AppliedAt { get; init; }

    public DateTimeOffset? ApprovedAt { get; init; }

    public DateTimeOffset? RejectedAt { get; init; }

    public string? RejectionReason { get; init; }
}