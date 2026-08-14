using System.Data;
using Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;
using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;
using Atlantis.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;

public sealed class CortexJobAvailabilityService
{
    private readonly AtlantisDbContext _dbContext;

    public CortexJobAvailabilityService(
        AtlantisDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CortexJobAvailabilityResult>
        SetAvailabilityAsync(
            Guid workerCortexJobQualificationId,
            bool isAvailable,
            DateTimeOffset now,
            CancellationToken cancellationToken = default)
    {
        if (workerCortexJobQualificationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Worker Cortex job qualification ID " +
                "cannot be empty.",
                nameof(workerCortexJobQualificationId));
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.ReadCommitted,
                cancellationToken);

        try
        {
            var qualificationExists =
                await _dbContext
                    .WorkerCortexJobQualifications
                    .AnyAsync(
                        entity =>
                            entity.Id ==
                            workerCortexJobQualificationId,
                        cancellationToken);

            if (!qualificationExists)
            {
                throw new
                    CortexJobQualificationNotFoundException(
                        workerCortexJobQualificationId);
            }

            var availability =
                await _dbContext
                    .WorkerCortexJobAvailability
                    .FromSqlInterpolated(
                        $"""
                        SELECT *
                        FROM worker_cortex_job_availability
                        WHERE worker_cortex_job_qualification_id =
                            {workerCortexJobQualificationId}
                        FOR UPDATE
                        """)
                    .SingleOrDefaultAsync(
                        cancellationToken);

            if (availability is null)
            {
                throw new
                    WorkerCortexJobAvailabilityNotFoundException(
                        workerCortexJobQualificationId);
            }

            if (availability.IsAvailable == isAvailable)
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return new CortexJobAvailabilityResult
                {
                    WorkerCortexJobQualificationId =
                        workerCortexJobQualificationId,

                    IsAvailable =
                        availability.IsAvailable,

                    StateChanged =
                        false,

                    ChangedAt =
                        availability.ChangedAt
                };
            }

            availability.IsAvailable =
                isAvailable;

            availability.ChangedAt =
                now;

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return new CortexJobAvailabilityResult
            {
                WorkerCortexJobQualificationId =
                    workerCortexJobQualificationId,

                IsAvailable =
                    availability.IsAvailable,

                StateChanged =
                    true,

                ChangedAt =
                    availability.ChangedAt
            };
        }
        catch
        {
            await transaction.RollbackAsync(
                CancellationToken.None);

            throw;
        }
    }
}