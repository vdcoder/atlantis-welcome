namespace Atlantis.Api.Persistence.Records;

public sealed class CortexJobApplicationRecord
{
    public Guid Id { get; set; }

    public Guid CortexJobDefinitionId { get; set; }

    public required string WorkerCitizenId { get; set; }

    public required string DepositAccountId { get; set; }

    public int Status { get; set; }

    public DateTimeOffset AppliedAt { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public DateTimeOffset? RejectedAt { get; set; }

    public string? RejectionReason { get; set; }

    public CortexJobDefinitionRecord CortexJobDefinition
    {
        get;
        set;
    } = null!;

    public WorkerCortexJobQualificationRecord?
        WorkerCortexJobQualification
    {
        get;
        set;
    }
}