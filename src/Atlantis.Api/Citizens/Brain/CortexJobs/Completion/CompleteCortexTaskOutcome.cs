namespace Atlantis.Api.Citizens.Brain.CortexJobs.Completion
{
    public enum CompleteCortexTaskOutcome
    {
        Completed = 1,
        InvalidResult = 2,
        AssignmentNotFound = 3,
        AssignmentNotOwnedByWorker = 4,
        AssignmentNotPrimed = 5,
        UnsupportedTaskType = 6
    }
}
