namespace Atlantis.Api.Persistence.Records;

public sealed class CortexTaskAssignmentRecord
{
    public Guid Id { get; set; }

    public Guid CortexTaskId { get; set; }

    public Guid WorkerCortexJobQualificationId { get; set; }

    public string WorkerCitizenId { get; set; } = null!;

    public int Status { get; set; }

    public DateTimeOffset AssignedAt { get; set; }

    public DateTimeOffset FirstPrimedAt { get; set; }

    public DateTimeOffset LastPrimedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public DateTimeOffset? FailedAt { get; set; }

    public DateTimeOffset? CancelledAt { get; set; }

    public DateTimeOffset? PaidAt { get; set; }

    public string? FailureReason { get; set; }

    public Guid? PaymentTransactionId { get; set; }

    public CortexTaskRecord CortexTask
    {
        get;
        set;
    } = null!;

    public WorkerCortexJobQualificationRecord
        WorkerCortexJobQualification
    {
        get;
        set;
    } = null!;

    public CortexTaskResultRecord? Result
    {
        get;
        set;
    }
}