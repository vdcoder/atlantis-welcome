namespace Atlantis.Api.Persistence.Entities;

public sealed class WorkerCortexJobAvailabilityEntity
{
    public Guid WorkerCortexJobQualificationId { get; set; }

    public bool IsAvailable { get; set; }

    public DateTimeOffset ChangedAt { get; set; }

    public WorkerCortexJobQualificationEntity Qualification
    {
        get;
        set;
    } = null!;
}