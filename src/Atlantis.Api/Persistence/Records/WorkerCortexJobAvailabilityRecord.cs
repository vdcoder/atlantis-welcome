namespace Atlantis.Api.Persistence.Records;

public sealed class WorkerCortexJobAvailabilityRecord
{
    public Guid WorkerCortexJobQualificationId { get; set; }

    public bool IsAvailable { get; set; }

    public DateTimeOffset ChangedAt { get; set; }

    public WorkerCortexJobQualificationRecord Qualification
    {
        get;
        set;
    } = null!;
}