namespace Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;

public sealed record CortexTaskAssignment
{
    public required Guid Id { get; init; }

    public required Guid CortexTaskId { get; init; }

    public required Guid WorkerCortexJobQualificationId { get; init; }

    public required string WorkerCitizenId { get; init; }

    public required CortexTaskAssignmentStatus Status
    {
        get;
        init;
    }

    public required DateTimeOffset AssignedAt { get; init; }

    public required DateTimeOffset FirstPrimedAt { get; init; }

    public required DateTimeOffset LastPrimedAt { get; init; }

    public DateTimeOffset? CompletedAt { get; init; }

    public DateTimeOffset? FailedAt { get; init; }

    public DateTimeOffset? CancelledAt { get; init; }

    public DateTimeOffset? PaidAt { get; init; }

    public string? FailureReason { get; init; }

    public Guid? PaymentTransactionId { get; init; }
}