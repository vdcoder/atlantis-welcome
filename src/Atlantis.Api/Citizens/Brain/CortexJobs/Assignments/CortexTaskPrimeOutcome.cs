namespace Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;

public enum CortexTaskPrimeOutcome
{
    Primed = 1,
    QualificationNotApproved = 2,
    WorkerUnavailableForQualification = 3,
    CortexJobDefinitionInactive = 4,
    NoEligibleTask = 5
}