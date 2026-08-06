namespace Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;

public enum CortexTaskAssignmentStatus
{
    Primed = 1,
    Completed = 2,
    // Value 3 retired; formerly Paid.
    Failed = 4,
    Cancelled = 5
}