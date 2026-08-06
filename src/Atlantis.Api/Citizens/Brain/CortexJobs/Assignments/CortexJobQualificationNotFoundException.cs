namespace Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;

public sealed class
    CortexJobQualificationNotFoundException
    : Exception
{
    public CortexJobQualificationNotFoundException(
        Guid qualificationId)
        : base(
            $"Cortex job qualification " +
            $"'{qualificationId}' was not found.")
    {
        QualificationId = qualificationId;
    }

    public Guid QualificationId
    {
        get;
    }
}