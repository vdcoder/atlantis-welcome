using Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;
using Atlantis.Api.Citizens.Brain.CortexJobs.Tasks;
using Atlantis.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Citizens.Brain.CortexJobs.Context;

public sealed class PrimedCortexTaskContextLoader
    : IPrimedCortexTaskContextLoader
{
    private readonly AtlantisDbContext _dbContext;

    public PrimedCortexTaskContextLoader(
        AtlantisDbContext dbContext)
    {
        _dbContext =
            dbContext ??
            throw new ArgumentNullException(
                nameof(dbContext));
    }

    public async Task<PrimedCortexTaskContext?> LoadAsync(
        string workerCitizenId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            workerCitizenId);

        var primedTask =
            await (
                from assignment in
                    _dbContext.CortexTaskAssignments
                        .AsNoTracking()

                join task in
                    _dbContext.CortexTasks
                        .AsNoTracking()
                    on assignment.CortexTaskId
                    equals task.Id

                where
                    assignment.WorkerCitizenId ==
                        workerCitizenId &&
                    assignment.Status ==
                        (int)CortexTaskAssignmentStatus.Primed

                select new
                {
                    AssignmentId =
                        assignment.Id,

                    CortexTaskId =
                        task.Id,

                    task.Instructions
                })
                .SingleOrDefaultAsync(
                    cancellationToken);

        if (primedTask is null)
        {
            return null;
        }

        var simulationTaskExists =
            await _dbContext.SimulateCitizenPassTasks
                .AsNoTracking()
                .AnyAsync(
                    entity =>
                        entity.CortexTaskId ==
                            primedTask.CortexTaskId,
                    cancellationToken);

        if (!simulationTaskExists)
        {
            throw new InvalidOperationException(
                $"Primed Cortex task " +
                $"'{primedTask.CortexTaskId}' has no supported " +
                "specialized task payload.");
        }

        if (string.IsNullOrWhiteSpace(
                primedTask.Instructions))
        {
            throw new InvalidOperationException(
                $"Primed Cortex task " +
                $"'{primedTask.CortexTaskId}' has empty instructions.");
        }

        return new PrimedCortexTaskContext(
            assignmentId:
                primedTask.AssignmentId,

            cortexTaskId:
                primedTask.CortexTaskId,

            cortexTaskType:
                CortexTaskResultTypes.SimulateCitizenPass,

            instructionsAndInputs:
                primedTask.Instructions);
    }
}