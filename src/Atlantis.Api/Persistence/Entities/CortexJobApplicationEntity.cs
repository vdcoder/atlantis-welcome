namespace Atlantis.Api.Persistence.Entities;

public sealed class CortexJobApplicationEntity
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

    public CortexJobDefinitionEntity CortexJobDefinition
    {
        get;
        set;
    } = null!;

    public WorkerCortexJobQualificationEntity?
        WorkerCortexJobQualification
    {
        get;
        set;
    }
}