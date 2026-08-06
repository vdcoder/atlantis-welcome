using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;

namespace Atlantis.Api.Citizens.Brain.CortexJobs.Domain.ScheduleConditions;

public sealed record AllowedLocationsCondition
    : CortexJobScheduleCondition
{
    public required IReadOnlySet<string>
        AllowedLocationEntityIds
    {
        get;
        init;
    }

    public override bool PassesCondition(
        CortexJobScheduleContext context)
    {
        if (context.WorkerLocationEntityId is null)
        {
            return false;
        }

        return AllowedLocationEntityIds.Contains(
            context.WorkerLocationEntityId);
    }
}