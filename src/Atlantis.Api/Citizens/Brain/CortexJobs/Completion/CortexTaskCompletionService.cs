using Atlantis.Api.Citizens.Brain.CortexJobs
    .Assignments;
using Atlantis.Api.Citizens.Brain.CortexJobs
    .Feedback;
using Atlantis.Api.Citizens.Brain.CortexJobs
    .Inbox;
using Atlantis.Api.Citizens.Brain.CortexJobs.Remuneration;
using Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation;
using Atlantis.Api.Citizens.Brain.CortexJobs.Simulation.WorkOrders;
using Atlantis.Api.Citizens.Brain.CortexJobs.Tasks;
using Atlantis.Api.Persistence;
using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Completion;

public sealed class CortexTaskCompletionService
{
    private readonly AtlantisDbContext _dbContext;

    private readonly SimulateCitizenPassResultValidator
        _simulationValidator;

    private readonly CortexTaskFeedbackService
        _feedbackService;

    public CortexTaskCompletionService(
        AtlantisDbContext dbContext,
        SimulateCitizenPassResultValidator simulationValidator,
        CortexTaskFeedbackService feedbackService)
    {
        _dbContext =
            dbContext ??
            throw new ArgumentNullException(
                nameof(dbContext));

        _simulationValidator =
            simulationValidator ??
            throw new ArgumentNullException(
                nameof(simulationValidator));

        _feedbackService =
            feedbackService ??
            throw new ArgumentNullException(
                nameof(feedbackService));
    }

    public async Task<CompleteCortexTaskResult>
        CompleteAsync(
            CompleteCortexTaskRequest request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.WorkerCitizenId);

        if (request.AssignmentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Assignment ID cannot be empty.",
                nameof(request));
        }

        var completedAt =
            DateTimeOffset.UtcNow;

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

        try
        {
            var assignment =
                await _dbContext.CortexTaskAssignments
                    .FromSqlInterpolated(
                        $"""
                        SELECT *
                        FROM cortex_task_assignments
                        WHERE id = {request.AssignmentId}
                        FOR UPDATE
                        """)
                    .SingleOrDefaultAsync(
                        cancellationToken);

            if (assignment is null)
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return new CompleteCortexTaskResult
                    {
                        Outcome =
                            CompleteCortexTaskOutcome
                                .AssignmentNotFound
                    };
            }

            if (!string.Equals(
                    assignment.WorkerCitizenId,
                    request.WorkerCitizenId,
                    StringComparison.Ordinal))
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return new CompleteCortexTaskResult
                    {
                        Outcome =
                        CompleteCortexTaskOutcome
                            .AssignmentNotOwnedByWorker
                    };
            }

            if (assignment.Status !=
                (int)CortexTaskAssignmentStatus.Primed)
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return new CompleteCortexTaskResult
                    {
                        Outcome =
                        CompleteCortexTaskOutcome
                            .AssignmentNotPrimed
                    };
            }

            var task =
                await _dbContext.CortexTasks
                    .Include(
                        entity =>
                            entity.CortexJobDefinition)
                    .SingleAsync(
                        entity =>
                            entity.Id ==
                            assignment.CortexTaskId,
                        cancellationToken);

            var simulationTask =
                await _dbContext
                    .SimulateCitizenPassTasks
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        entity =>
                            entity.CortexTaskId ==
                            task.Id,
                        cancellationToken);

            if (simulationTask is null)
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return new CompleteCortexTaskResult
                    {
                        Outcome =
                        CompleteCortexTaskOutcome
                            .UnsupportedTaskType
                    };
            }

            var validation =
                _simulationValidator.Validate(
                    request.ResultSerialized);

            if (!validation.IsValid)
            {
                await transaction.CommitAsync(
                    cancellationToken);

                await _feedbackService
                    .CreateInvalidCompletionFormatSayAsync(
                        request.WorkerCitizenId,
                        validation.Message ??
                            "The result was invalid.",
                        completedAt);

                return new CompleteCortexTaskResult
                    {
                        Outcome =
                        CompleteCortexTaskOutcome
                            .InvalidResult,

                        ValidationMessage =
                        validation.Message
                    };
            }

            var resultId =
                Guid.NewGuid();

            var result =
                new CortexTaskResultRecord
                {
                    Id = resultId,

                    CortexTaskAssignmentId =
                        assignment.Id,

                    ResultType =
                        CortexTaskResultTypes
                            .SimulateCitizenPass,

                    ResultSerialized =
                        request.ResultSerialized,

                    CompletedAt =
                        completedAt
                };

            var inboxMessage =
                new CortexJobInboxMessageRecord
                {
                    Id =
                        Guid.NewGuid(),

                    InboxId =
                        task.CompletionInboxId,

                    CortexTaskId =
                        task.Id,

                    CortexTaskAssignmentId =
                        assignment.Id,

                    Type =
                        (int)CortexJobInboxMessageType
                            .CortexTaskCompleted,

                    PayloadType =
                        CortexTaskResultTypes
                            .SimulateCitizenPass,

                    PayloadSerialized =
                        request.ResultSerialized,

                    CreatedAt =
                        completedAt
                };

            var remuneration =
                new CortexTaskRemunerationRecord
                {
                    Id =
                        Guid.NewGuid(),

                    CortexTaskId =
                        task.Id,

                    CortexTaskAssignmentId =
                        assignment.Id,

                    WorkerCitizenId =
                        request.WorkerCitizenId,

                    EmployerAccountId =
                        task.CortexJobDefinition
                            .EmployerAccountId,

                    Amount =
                        task.Reward,

                    Currency =
                        task.Currency,

                    Status =
                        (int)
                        CortexTaskRemunerationStatus
                            .Pending,

                    CreatedAt =
                        completedAt,

                    PaidAt =
                        null,

                    LedgerTransactionId =
                        null
                };

            var workOrder =
                await _dbContext
                    .SimulateCitizenPassWorkOrders
                    .SingleOrDefaultAsync(
                        entity =>
                            entity.CortexTaskId ==
                                task.Id,
                        cancellationToken);

            assignment.Status =
                (int)CortexTaskAssignmentStatus.Completed;

            assignment.CompletedAt =
                completedAt;

            task.Status =
                (int)CortexTaskStatus.Completed;

            if (workOrder is not null)
            {
                workOrder.Status =
                    (int)
                    SimulateCitizenPassWorkOrderStatus
                        .Completed;

                workOrder.CompletedAt =
                    completedAt;
            }

            _dbContext.CortexTaskResults.Add(
                result);

            _dbContext.CortexJobInboxMessages.Add(
                inboxMessage);

            _dbContext.CortexTaskRemunerations.Add(
                remuneration);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return new CompleteCortexTaskResult
                {
                    Outcome =
                    CompleteCortexTaskOutcome.Completed,

                    CortexTaskResultId =
                    resultId
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