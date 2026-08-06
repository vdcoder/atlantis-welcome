using System.Text.Json;
using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;
using Atlantis.Api.Citizens.Brain.CortexJobs.Domain.ScheduleConditions;
using Atlantis.Api.Persistence.Entities;

namespace Atlantis.Api.Persistence.Mappers;

public static class CortexJobDefinitionMapper
{
    public static CortexJobDefinitionEntity ToEntity(
        CortexJobDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(
            definition);

        var entity = new CortexJobDefinitionEntity
        {
            Id = definition.Id,
            EmployerAccountId =
                definition.EmployerAccountId,
            Name = definition.Name,
            Description = definition.Description,
            Qualifications =
                definition.Qualifications,
            CompletionInboxId =
                definition.CompletionInboxId,
            FailureInboxId =
                definition.FailureInboxId,
            CreatedAt = definition.CreatedAt,
            DeactivatedAt =
                definition.DeactivatedAt
        };

        entity.ScheduleConditions =
            definition.Schedule.Conditions
                .Select(
                    (condition, index) =>
                        ToConditionEntity(
                            definition.Id,
                            condition,
                            index))
                .ToList();

        return entity;
    }

    public static CortexJobDefinition ToDomain(
        CortexJobDefinitionEntity entity)
    {
        ArgumentNullException.ThrowIfNull(
            entity);

        var conditions = entity.ScheduleConditions
            .OrderBy(condition =>
                condition.SortOrder)
            .Select(ToConditionDomain)
            .ToList();

        return new CortexJobDefinition
        {
            Id = entity.Id,
            EmployerAccountId =
                entity.EmployerAccountId,
            Name = entity.Name,
            Description = entity.Description,
            Qualifications =
                entity.Qualifications,
            Schedule = new CortexJobSchedule
            {
                Conditions = conditions
            },
            CompletionInboxId =
                entity.CompletionInboxId,
            FailureInboxId =
                entity.FailureInboxId,
            CreatedAt = entity.CreatedAt,
            DeactivatedAt =
                entity.DeactivatedAt
        };
    }

    private static CortexJobScheduleConditionEntity
        ToConditionEntity(
            Guid cortexJobDefinitionId,
            CortexJobScheduleCondition condition,
            int sortOrder)
    {
        var conditionType =
            GetConditionType(condition);

        var configurationJson =
            JsonSerializer.Serialize(
                condition,
                condition.GetType());

        return new CortexJobScheduleConditionEntity
        {
            Id = Guid.NewGuid(),
            CortexJobDefinitionId = cortexJobDefinitionId,
            ConditionType = conditionType,
            ConfigurationJson = configurationJson,
            SortOrder = sortOrder
        };
    }

    private static CortexJobScheduleCondition
        ToConditionDomain(
            CortexJobScheduleConditionEntity entity)
    {
        return entity.ConditionType switch
        {
            CortexJobScheduleConditionTypes.AllowedDays =>
                Deserialize<AllowedDaysCondition>(
                    entity),

            CortexJobScheduleConditionTypes
                .AllowedLocations =>
                Deserialize<AllowedLocationsCondition>(
                    entity),

            CortexJobScheduleConditionTypes.TimeRange =>
                Deserialize<TimeRangeCondition>(
                    entity),

            _ => throw new InvalidOperationException(
                $"Unknown job schedule condition " +
                $"type '{entity.ConditionType}'.")
        };
    }

    private static string GetConditionType(
        CortexJobScheduleCondition condition)
    {
        return condition switch
        {
            AllowedDaysCondition =>
                CortexJobScheduleConditionTypes.AllowedDays,

            AllowedLocationsCondition =>
                CortexJobScheduleConditionTypes
                    .AllowedLocations,

            TimeRangeCondition =>
                CortexJobScheduleConditionTypes.TimeRange,

            _ => throw new InvalidOperationException(
                $"Unsupported job schedule condition " +
                $"'{condition.GetType().Name}'.")
        };
    }

    private static T Deserialize<T>(
        CortexJobScheduleConditionEntity entity)
        where T : CortexJobScheduleCondition
    {
        return JsonSerializer.Deserialize<T>(
                entity.ConfigurationJson)
            ?? throw new InvalidOperationException(
                $"Unable to deserialize condition " +
                $"'{entity.Id}' as '{typeof(T).Name}'.");
    }
}