namespace Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;

public sealed record CortexJobAvailabilityResult
{
    public required Guid WorkerCortexJobQualificationId
    {
        get;
        init;
    }

    public required bool IsAvailable
    {
        get;
        init;
    }

    public required bool StateChanged
    {
        get;
        init;
    }

    public required DateTimeOffset ChangedAt
    {
        get;
        init;
    }
}