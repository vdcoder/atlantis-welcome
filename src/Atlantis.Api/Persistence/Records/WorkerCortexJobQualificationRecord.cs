namespace Atlantis.Api.Persistence.Records;

public sealed class WorkerCortexJobQualificationRecord
{
    public Guid Id { get; set; }

    public Guid CortexJobApplicationId { get; set; }

    public required string WorkerCitizenId { get; set; }

    public required string DepositAccountId { get; set; }

    public Guid CortexJobDefinitionId { get; set; }

    public int Status { get; set; }

    public DateTimeOffset QualifiedAt { get; set; }

    public DateTimeOffset? SuspendedAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    public CortexJobApplicationRecord CortexJobApplication
    {
        get;
        set;
    } = null!;

    public CortexJobDefinitionRecord CortexJobDefinition
    {
        get;
        set;
    } = null!;

    public WorkerCortexJobAvailabilityRecord? Availability
    {
        get;
        set;
    }
}