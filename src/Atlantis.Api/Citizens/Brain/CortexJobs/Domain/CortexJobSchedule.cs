namespace Atlantis.Api.Citizens.Brain.CortexJobs.Domain;

public sealed record CortexJobSchedule
{
    public IReadOnlyList<CortexJobScheduleCondition> Conditions
    {
        get;
        init;
    } = [];

    public bool PassesAllConditions(
        CortexJobScheduleContext context)
    {
        return Conditions.All(
            condition =>
                condition.PassesCondition(context));
    }
}