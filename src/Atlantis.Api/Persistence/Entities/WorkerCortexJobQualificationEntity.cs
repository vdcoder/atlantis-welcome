namespace Atlantis.Api.Persistence.Entities;

public sealed class WorkerCortexJobQualificationEntity
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

    public CortexJobApplicationEntity CortexJobApplication
    {
        get;
        set;
    } = null!;

    public CortexJobDefinitionEntity CortexJobDefinition
    {
        get;
        set;
    } = null!;

    public WorkerCortexJobAvailabilityEntity? Availability
    {
        get;
        set;
    }
}