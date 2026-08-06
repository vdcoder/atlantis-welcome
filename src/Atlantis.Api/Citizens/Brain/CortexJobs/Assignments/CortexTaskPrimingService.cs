using Atlantis.Api.Citizens.Brain.CortexJobs.Qualifications;
using Atlantis.Api.Citizens.Brain.CortexJobs.Tasks;
using Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;
using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;
using Atlantis.Api.Data;
using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;

public sealed class CortexTaskPrimingService
{
    private readonly AtlantisDbContext _dbContext;

    public CortexTaskPrimingService(
        AtlantisDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CortexTaskPrimeResult>
        TryPrimeNextEligibleCortexTaskAsync(
            Guid workerCortexJobQualificationId,
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
            /*
             * Lock order:
             *
             * 1. Qualification row
             * 2. Worker advisory transaction lock
             * 3. Worker availability row
             * 4. Job definition row
             * 5. Candidate task row
             *
             * Other services touching multiple Cortex resources
             * should preserve this order.
             */

            var qualification =
                await _dbContext
                    .WorkerCortexJobQualifications
                    .FromSqlInterpolated(
                        $"""
                        SELECT *
                        FROM worker_cortex_job_qualifications
                        WHERE id =
                            {workerCortexJobQualificationId}
                        FOR SHARE
                        """)
                    .SingleOrDefaultAsync(
                        cancellationToken);

            if (qualification is null)
            {
                throw new
                    CortexJobQualificationNotFoundException(
                        workerCortexJobQualificationId);
            }

            if (qualification.Status !=
                (int)WorkerCortexJobQualificationStatus.Approved)
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return CortexTaskPrimeResult.NotPrimed(
                    CortexTaskPrimeOutcome
                        .QualificationNotApproved);
            }

            var workerCitizenId = qualification.WorkerCitizenId;

            if (string.IsNullOrWhiteSpace(workerCitizenId))
            {
                throw new InvalidOperationException(
                    $"Qualification " +
                    $"'{workerCortexJobQualificationId}' " +
                    "has no worker citizen ID.");
            }

            await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"""
                SELECT pg_advisory_xact_lock(
                    hashtextextended({workerCitizenId}, 0)
                )
                """,
                cancellationToken);

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

            if (!availability.IsAvailable)
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return CortexTaskPrimeResult.NotPrimed(
                    CortexTaskPrimeOutcome
                        .WorkerUnavailableForQualification);
            }

            var definition =
                await _dbContext.CortexJobDefinitions
                    .FromSqlInterpolated(
                        $"""
                        SELECT *
                        FROM cortex_job_definitions
                        WHERE id =
                            {qualification.CortexJobDefinitionId}
                        FOR SHARE
                        """)
                    .SingleOrDefaultAsync(
                        cancellationToken);

            if (definition is null)
            {
                throw new InvalidOperationException(
                    $"Cortex job definition " +
                    $"'{qualification.CortexJobDefinitionId}' " +
                    $"referenced by qualification " +
                    $"'{workerCortexJobQualificationId}' " +
                    $"was not found.");
            }

            if (!definition.IsActiveAt(now))
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return CortexTaskPrimeResult.NotPrimed(
                    CortexTaskPrimeOutcome
                        .CortexJobDefinitionInactive);
            }

            var activeAssignment =
                await _dbContext.CortexTaskAssignments
                    .AsNoTracking()
                    .Where(
                        entity =>
                            entity
                                .WorkerCitizenId ==
                            workerCitizenId &&
                            entity.Status ==
                            (int)CortexTaskAssignmentStatus.Primed)
                    .Select(
                        entity =>
                            new
                            {
                                entity.Id,
                                entity.WorkerCortexJobQualificationId
                            })
                    .SingleOrDefaultAsync(
                        cancellationToken);

            if (activeAssignment is not null)
            {
                throw new ActiveCortexTaskAssignmentExistsException(
                    workerCitizenId,
                    activeAssignment.WorkerCortexJobQualificationId,
                    activeAssignment.Id);
            }

            var task =
                await _dbContext.CortexTasks
                    .FromSqlInterpolated(
                        $"""
                        SELECT *
                        FROM cortex_tasks
                        WHERE cortex_job_definition_id =
                            {qualification.CortexJobDefinitionId}
                          AND status =
                            {(int)CortexTaskStatus.Available}
                          AND available_at IS NOT NULL
                          AND available_at <= {now}
                          AND (
                                unavailable_at IS NULL
                                OR unavailable_at > {now}
                              )
                        ORDER BY
                            available_at ASC,
                            created_at ASC,
                            id ASC
                        FOR UPDATE SKIP LOCKED
                        LIMIT 1
                        """)
                    .SingleOrDefaultAsync(
                        cancellationToken);

            if (task is null)
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return CortexTaskPrimeResult.NotPrimed(
                    CortexTaskPrimeOutcome.NoEligibleTask);
            }

            var assignmentId =
                Guid.NewGuid();

            task.Status =
                (int)CortexTaskStatus.Assigned;

            var assignment =
                new CortexTaskAssignmentEntity
                {
                    Id =
                        assignmentId,

                    CortexTaskId =
                        task.Id,

                    WorkerCortexJobQualificationId =
                        workerCortexJobQualificationId,

                    WorkerCitizenId =
                        workerCitizenId,

                    Status =
                        (int)CortexTaskAssignmentStatus.Primed,

                    AssignedAt =
                        now,

                    FirstPrimedAt =
                        now,

                    LastPrimedAt =
                        now
                };

            _dbContext.CortexTaskAssignments.Add(
                assignment);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return CortexTaskPrimeResult.Primed(
                task.Id,
                assignmentId);
        }
        catch
        {
            await transaction.RollbackAsync(
                CancellationToken.None);

            throw;
        }
    }
}