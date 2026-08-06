using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;

namespace Atlantis.Api.Citizens.Brain.CortexJobs.Domain.ScheduleConditions;

public sealed record AllowedDaysCondition
    : CortexJobScheduleCondition
{
    public required IReadOnlySet<DayOfWeek> AllowedDays
    {
        get;
        init;
    }

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

        return AllowedDays.Contains(
            localTime.DayOfWeek);
    }
}