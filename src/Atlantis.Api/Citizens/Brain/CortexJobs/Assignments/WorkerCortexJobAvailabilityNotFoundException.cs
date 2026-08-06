namespace Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;

public sealed class
    WorkerCortexJobAvailabilityNotFoundException
    : Exception
{
    public WorkerCortexJobAvailabilityNotFoundException(
        Guid qualificationId)
        : base(
            $"Worker Cortex job availability for " +
            $"qualification '{qualificationId}' was not found.")
    {
        QualificationId = qualificationId;
    }

    public Guid QualificationId
    {
        get;
    }
}