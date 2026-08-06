using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;

namespace Atlantis.Api.Citizens.Brain.CortexJobs.Domain.ScheduleConditions;

public sealed record TimeRangeCondition
    : CortexJobScheduleCondition
{
    public required TimeOnly StartsAt { get; init; }

    public required TimeOnly EndsAt { get; init; }

    public required string TimeZoneId { get; init; }

    public override bool PassesCondition(
        CortexJobScheduleContext context)
    {
        var timeZone =
            TimeZoneInfo.FindSystemTimeZoneById(
                TimeZoneId);

        var localTime =
            TimeZoneInfo.ConvertTime(
                context.Now,
                timeZone);

        var localTimeOnly =
            TimeOnly.FromDateTime(
                localTime.DateTime);

        if (StartsAt <= EndsAt)
        {
            return localTimeOnly >= StartsAt &&
                   localTimeOnly < EndsAt;
        }

        // Overnight range, such as 22:00–06:00.
        return localTimeOnly >= StartsAt ||
               localTimeOnly < EndsAt;
    }
}