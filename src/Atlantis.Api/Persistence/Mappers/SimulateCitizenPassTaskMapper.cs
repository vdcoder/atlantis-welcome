using Atlantis.Api.Citizens.Brain.CortexJobs.Simulation;
using Atlantis.Api.Persistence.Records;

namespace Atlantis.Api.Persistence.Mappers;

public static class SimulateCitizenPassTaskMapper
{
    public static SimulateCitizenPassTaskRecord ToEntity(
        SimulateCitizenPassTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        return new SimulateCitizenPassTaskRecord
        {
            CortexTaskId = task.CortexTaskId,
            ExternalCitizenId =
                task.ExternalCitizenId,
            ExternalWorldId =
                task.ExternalWorldId,
            ExternalRequestId =
                task.ExternalRequestId,
            ExternalScenarioId =
                task.ExternalScenarioId,
            ReceivedAt = task.ReceivedAt
        };
    }

    public static SimulateCitizenPassTask ToDomain(
        SimulateCitizenPassTaskRecord entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new SimulateCitizenPassTask
        {
            CortexTaskId = entity.CortexTaskId,
            ExternalCitizenId =
                entity.ExternalCitizenId,
            ExternalWorldId =
                entity.ExternalWorldId,
            ExternalRequestId =
                entity.ExternalRequestId,
            ExternalScenarioId =
                entity.ExternalScenarioId,
            ReceivedAt = entity.ReceivedAt
        };
    }
}