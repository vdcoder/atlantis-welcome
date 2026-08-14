using Atlantis.Api.Citizens.Brain.CortexJobs.Tasks;
using Atlantis.Api.Persistence.Records;

namespace Atlantis.Api.Persistence.Mappers;

public static class CortexTaskMapper
{
    public static CortexTaskRecord ToEntity(
        CortexTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        return new CortexTaskRecord
        {
            Id = task.Id,
            CortexJobDefinitionId =
                task.CortexJobDefinitionId,
            Instructions = task.Instructions,
            Reward = task.Reward,
            Currency = task.Currency,
            CompletionInboxId =
                task.CompletionInboxId,
            FailureInboxId =
                task.FailureInboxId,
            Status = (int)task.Status,
            CreatedAt = task.CreatedAt,
            AvailableAt = task.AvailableAt,
            UnavailableAt = task.UnavailableAt
        };
    }

    public static CortexTask ToDomain(
        CortexTaskRecord entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new CortexTask
        {
            Id = entity.Id,
            CortexJobDefinitionId =
                entity.CortexJobDefinitionId,
            Instructions = entity.Instructions,
            Reward = entity.Reward,
            Currency = entity.Currency,
            CompletionInboxId =
                entity.CompletionInboxId,
            FailureInboxId =
                entity.FailureInboxId,
            Status = (CortexTaskStatus)entity.Status,
            CreatedAt = entity.CreatedAt,
            AvailableAt = entity.AvailableAt,
            UnavailableAt = entity.UnavailableAt
        };
    }
}