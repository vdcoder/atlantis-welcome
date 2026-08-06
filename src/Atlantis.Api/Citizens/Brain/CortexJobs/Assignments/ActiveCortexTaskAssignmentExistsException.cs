namespace Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;

public sealed class
    ActiveCortexTaskAssignmentExistsException
    : Exception
{
    public ActiveCortexTaskAssignmentExistsException(
        string workerCitizenId,
        Guid qualificationId,
        Guid assignmentId)
        : base(
            $"Worker citizen '{workerCitizenId}' already " +
            $"has active Cortex task assignment " +
            $"'{assignmentId}'.")
    {
        WorkerCitizenId = workerCitizenId;
        QualificationId = qualificationId;
        AssignmentId = assignmentId;
    }

    public string WorkerCitizenId
    {
        get;
    }

    public Guid QualificationId
    {
        get;
    }

    public Guid AssignmentId
    {
        get;
    }
}