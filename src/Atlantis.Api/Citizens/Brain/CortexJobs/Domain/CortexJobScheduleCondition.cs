namespace Atlantis.Api.Citizens.Brain.CortexJobs.Domain;

public abstract record CortexJobScheduleCondition
{
    public abstract bool PassesCondition(
        CortexJobScheduleContext context);
}