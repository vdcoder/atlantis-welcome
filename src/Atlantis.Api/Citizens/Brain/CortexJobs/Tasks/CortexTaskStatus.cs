namespace Atlantis.Api.Citizens.Brain.CortexJobs.Tasks;

public enum CortexTaskStatus
{
    Created = 1,
    Available = 2,
    Assigned = 3,
    Unavailable = 4,
    Completed = 5
}