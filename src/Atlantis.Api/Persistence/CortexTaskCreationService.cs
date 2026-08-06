using Atlantis.Api.Citizens.Brain.CortexJobs.Simulation;
using Atlantis.Api.Data;
using Atlantis.Api.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Persistence;

public sealed class CortexTaskCreationService
{
    private readonly AtlantisDbContext _dbContext;

    public CortexTaskCreationService(
        AtlantisDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool>
        TryCreateSimulateCitizenPassTaskAsync(
            SimulateCitizenPassTaskCreation creation,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(creation);

        var simulationTask =
            creation.SimulationTask;

        var alreadyExists =
            await _dbContext
                .SimulateCitizenPassTasks
                .AnyAsync(
                    entity =>
                        entity.ExternalWorldId ==
                            simulationTask.ExternalWorldId &&
                        entity.ExternalRequestId ==
                            simulationTask.ExternalRequestId,
                    cancellationToken);

        if (alreadyExists)
        {
            return false;
        }

        var cortexTaskEntity =
            CortexTaskMapper.ToEntity(
                creation.CortexTask);

        var simulationTaskEntity =
            SimulateCitizenPassTaskMapper.ToEntity(
                simulationTask);

        cortexTaskEntity.SimulateCitizenPassTask =
            simulationTaskEntity;

        _dbContext.CortexTasks.Add(
            cortexTaskEntity);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}