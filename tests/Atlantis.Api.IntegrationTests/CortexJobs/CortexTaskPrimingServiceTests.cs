using Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;
using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;
using Atlantis.Api.Citizens.Brain.CortexJobs.Onboarding;
using Atlantis.Api.Citizens.Brain.CortexJobs.Qualifications;
using Atlantis.Api.Citizens.Brain.CortexJobs.Tasks;
using Atlantis.Api.Data;
using Atlantis.Api.Economy.Accounts;
using Atlantis.Api.IntegrationTests.Infrastructure;
using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.IntegrationTests.CortexJobs;

public sealed class CortexTaskPrimingServiceTests
    : IsolatedDatabaseTest
{
    private static readonly Guid QualificationId =
        Guid.Parse(
            "d4767d3d-12b6-44ae-90ac-450f669fd201");

    private static readonly Guid DefinitionId =
        Guid.Parse(
            "46bb41ca-28b6-47e3-a6fe-6e909898dd01");

    private static readonly Guid TaskId =
        KnownCortexTasks.FirstSimulateCitizenPassTaskId;

    private const string WorkerCitizenId =
        "orestes";

    private static readonly Guid SecondDefinitionId =
        Guid.Parse(
            "81000000-0000-0000-0000-000000000001");

    private static readonly Guid SecondApplicationId =
        Guid.Parse(
            "81000000-0000-0000-0000-000000000002");

    private static readonly Guid SecondQualificationId =
        Guid.Parse(
            "81000000-0000-0000-0000-000000000003");

    private static readonly Guid SecondTaskId =
        Guid.Parse(
            "81000000-0000-0000-0000-000000000004");

    [Fact]
    public async Task
        TryPrime_WhenWorkerUnavailable_ReturnsUnavailableAndChangesNothing()
    {
        await using var dbContext =
            CreateDbContext();

        var now =
            await GetEligibleTimeAsync(
                dbContext);

        await SetAvailabilityAsync(
            dbContext,
            isAvailable: false,
            changedAt: now.AddMinutes(-1));

        var service =
            new CortexTaskPrimingService(
                dbContext);

        var result =
            await service
                .TryPrimeNextEligibleCortexTaskAsync(
                    QualificationId,
                    now);

        Assert.Equal(
            CortexTaskPrimeOutcome
                .WorkerUnavailableForQualification,
            result.Outcome);

        Assert.False(result.WasPrimed);
        Assert.Null(result.CortexTaskId);
        Assert.Null(result.CortexTaskAssignmentId);

        dbContext.ChangeTracker.Clear();

        var task =
            await dbContext.CortexTasks
                .AsNoTracking()
                .SingleAsync(
                    entity =>
                        entity.Id == TaskId);

        Assert.Equal(
            (int)CortexTaskStatus.Available,
            task.Status);

        Assert.False(
            await dbContext.CortexTaskAssignments
                .AnyAsync());
    }

    [Fact]
    public async Task
        TryPrime_WhenDefinitionIsInactive_ReturnsInactiveAndChangesNothing()
    {
        await using var dbContext =
            CreateDbContext();

        var now =
            await GetEligibleTimeAsync(
                dbContext);

        await SetAvailabilityAsync(
            dbContext,
            isAvailable: true,
            changedAt: now.AddMinutes(-1));

        await SetDefinitionDeactivatedAtAsync(
            dbContext,
            deactivatedAt: now);

        var service =
            new CortexTaskPrimingService(
                dbContext);

        var result =
            await service
                .TryPrimeNextEligibleCortexTaskAsync(
                    QualificationId,
                    now);

        Assert.Equal(
            CortexTaskPrimeOutcome
                .CortexJobDefinitionInactive,
            result.Outcome);

        Assert.False(result.WasPrimed);

        dbContext.ChangeTracker.Clear();

        var task =
            await dbContext.CortexTasks
                .AsNoTracking()
                .SingleAsync(
                    entity =>
                        entity.Id == TaskId);

        Assert.Equal(
            (int)CortexTaskStatus.Available,
            task.Status);

        Assert.False(
            await dbContext.CortexTaskAssignments
                .AnyAsync());
    }

    [Fact]
    public async Task
        TryPrime_WhenEligible_AssignsTaskAndCreatesPrimedAssignment()
    {
        await using var dbContext =
            CreateDbContext();

        var now =
            await GetEligibleTimeAsync(
                dbContext);

        await SetAvailabilityAsync(
            dbContext,
            isAvailable: true,
            changedAt: now.AddMinutes(-1));

        var service =
            new CortexTaskPrimingService(
                dbContext);

        var result =
            await service
                .TryPrimeNextEligibleCortexTaskAsync(
                    QualificationId,
                    now);

        Assert.Equal(
            CortexTaskPrimeOutcome.Primed,
            result.Outcome);

        Assert.True(result.WasPrimed);

        Assert.Equal(
            TaskId,
            result.CortexTaskId);

        Assert.NotNull(
            result.CortexTaskAssignmentId);

        dbContext.ChangeTracker.Clear();

        var task =
            await dbContext.CortexTasks
                .AsNoTracking()
                .SingleAsync(
                    entity =>
                        entity.Id == TaskId);

        Assert.Equal(
            (int)CortexTaskStatus.Assigned,
            task.Status);

        var assignment =
            await dbContext.CortexTaskAssignments
                .AsNoTracking()
                .SingleAsync();

        Assert.Equal(
            result.CortexTaskAssignmentId,
            assignment.Id);

        Assert.Equal(
            TaskId,
            assignment.CortexTaskId);

        Assert.Equal(
            QualificationId,
            assignment
                .WorkerCortexJobQualificationId);

        Assert.Equal(
            WorkerCitizenId,
            assignment.WorkerCitizenId);

        Assert.Equal(
            (int)CortexTaskAssignmentStatus.Primed,
            assignment.Status);

        Assert.Equal(now, assignment.AssignedAt);
        Assert.Equal(now, assignment.FirstPrimedAt);
        Assert.Equal(now, assignment.LastPrimedAt);

        Assert.Null(assignment.CompletedAt);
        Assert.Null(assignment.FailedAt);
        Assert.Null(assignment.CancelledAt);
        Assert.Null(assignment.PaidAt);
        Assert.Null(assignment.FailureReason);
        Assert.Null(assignment.PaymentTransactionId);

        var availability =
            await dbContext.WorkerCortexJobAvailability
                .AsNoTracking()
                .SingleAsync(
                    entity =>
                        entity
                            .WorkerCortexJobQualificationId ==
                        QualificationId);

        Assert.True(availability.IsAvailable);
    }

    [Fact]
    public async Task
        TryPrime_WhenActiveAssignmentExists_ThrowsAndDoesNotCreateAnother()
    {
        await using var dbContext =
            CreateDbContext();

        var now =
            await GetEligibleTimeAsync(
                dbContext);

        await SetAvailabilityAsync(
            dbContext,
            isAvailable: true,
            changedAt: now.AddMinutes(-1));

        var existingAssignmentId =
            Guid.NewGuid();

        var task =
            await dbContext.CortexTasks
                .SingleAsync(
                    entity =>
                        entity.Id == TaskId);

        task.Status =
            (int)CortexTaskStatus.Assigned;

        dbContext.CortexTaskAssignments.Add(
            new CortexTaskAssignmentEntity
            {
                Id = existingAssignmentId,

                CortexTaskId = TaskId,

                WorkerCortexJobQualificationId =
                    QualificationId,

                WorkerCitizenId =
                    WorkerCitizenId,

                Status =
                    (int)CortexTaskAssignmentStatus.Primed,

                AssignedAt = now,
                FirstPrimedAt = now,
                LastPrimedAt = now
            });

        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        var service =
            new CortexTaskPrimingService(
                dbContext);

        var exception =
            await Assert.ThrowsAsync<
                ActiveCortexTaskAssignmentExistsException>(
                () =>
                    service
                        .TryPrimeNextEligibleCortexTaskAsync(
                            QualificationId,
                            now.AddMinutes(1)));

        Assert.Equal(
            WorkerCitizenId,
            exception.WorkerCitizenId);

        Assert.Equal(
            QualificationId,
            exception.QualificationId);

        Assert.Equal(
            existingAssignmentId,
            exception.AssignmentId);

        Assert.Equal(
            1,
            await dbContext.CortexTaskAssignments
                .CountAsync());
    }

    [Fact]
    public async Task
    TryPrime_WhenDifferentQualificationForSameWorkerAlreadyHasPrimedTask_Throws()
    {
        await using var dbContext =
            CreateDbContext();

        var now =
            await GetEligibleTimeAsync(
                dbContext);

        await SetAvailabilityAsync(
            dbContext,
            isAvailable: true,
            changedAt: now.AddMinutes(-1));

        await CreateSecondQualificationAndTaskAsync(
            dbContext,
            now);

        var service =
            new CortexTaskPrimingService(
                dbContext);

        var firstResult =
            await service
                .TryPrimeNextEligibleCortexTaskAsync(
                    QualificationId,
                    now);

        Assert.True(firstResult.WasPrimed);
        Assert.NotNull(firstResult.CortexTaskAssignmentId);

        var firstAssignmentId =
            firstResult.CortexTaskAssignmentId.Value;

        dbContext.ChangeTracker.Clear();

        var exception =
            await Assert.ThrowsAsync<
                ActiveCortexTaskAssignmentExistsException>(
                () =>
                    service
                        .TryPrimeNextEligibleCortexTaskAsync(
                            SecondQualificationId,
                            now.AddSeconds(1)));

        Assert.Equal(
            firstAssignmentId,
            exception.AssignmentId);

        Assert.Equal(
            WorkerCitizenId,
            exception.WorkerCitizenId);

        Assert.Equal(
            QualificationId,
            exception.QualificationId);

        var assignments =
            await dbContext.CortexTaskAssignments
                .AsNoTracking()
                .Where(entity =>
                    entity.WorkerCitizenId == WorkerCitizenId &&
                    entity.Status ==
                        (int)CortexTaskAssignmentStatus.Primed)
                .ToListAsync();

        Assert.Single(assignments);

        var secondTask =
            await dbContext.CortexTasks
                .AsNoTracking()
                .SingleAsync(entity =>
                    entity.Id == SecondTaskId);

        Assert.Equal(
            (int)CortexTaskStatus.Available,
            secondTask.Status);
    }

    private static async Task
        CreateSecondQualificationAndTaskAsync(
            AtlantisDbContext dbContext,
            DateTimeOffset now)
    {
        var definition =
            new CortexJobDefinitionEntity
            {
                Id = SecondDefinitionId,

                EmployerAccountId =
                    KnownMoneyAccounts.AtlantisDevelopmentFundId,

                Name =
                    "Second Job Definition",

                Description =
                    "Description for the second job definition",

                Qualifications =
                    "Qualifications for the second job definition.",

                CompletionInboxId =
                    "inbox:test:second-job:completed",

                FailureInboxId =
                    "inbox:test:second-job:failed",

                CreatedAt = now,
                DeactivatedAt = null
            };

        var application =
            new CortexJobApplicationEntity
            {
                Id = SecondApplicationId,
                CortexJobDefinitionId = SecondDefinitionId,
                WorkerCitizenId = WorkerCitizenId,

                DepositAccountId =
                    KnownMoneyAccounts.OrestesId,

                Status =
                    (int)CortexJobApplicationStatus.Approved,

                AppliedAt = now,
                ApprovedAt = now
            };

        var qualification =
            new WorkerCortexJobQualificationEntity
            {
                Id = SecondQualificationId,
                CortexJobDefinitionId = SecondDefinitionId,
                CortexJobApplicationId = SecondApplicationId,
                WorkerCitizenId = WorkerCitizenId,

                DepositAccountId =
                    KnownMoneyAccounts.OrestesId,

                Status =
                     (int)WorkerCortexJobQualificationStatus.Approved,

                QualifiedAt = now
            };

        var availability =
            new WorkerCortexJobAvailabilityEntity
            {
                WorkerCortexJobQualificationId =
                    SecondQualificationId,

                IsAvailable = true,
                ChangedAt = now
            };

        var task =
            new CortexTaskEntity
            {
                Id = SecondTaskId,
                CortexJobDefinitionId = SecondDefinitionId,

                Instructions =
                    """
                {
                  "cortexTaskType": "TestSecondQualification",
                  "instructionsAndInputs": {
                    "message": "Second qualification test task."
                  }
                }
                """,

                Reward = 1.00m,
                Currency = "ATC",

                CompletionInboxId =
                    "inbox:test:second-job:completed",

                FailureInboxId =
                    "inbox:test:second-job:failed",

                Status =
                    (int)CortexTaskStatus.Available,

                CreatedAt = now,
                AvailableAt = now,
                UnavailableAt = null
            };

        dbContext.CortexJobDefinitions.Add(
            definition);

        dbContext.CortexJobApplications.Add(
            application);

        dbContext.WorkerCortexJobQualifications.Add(
            qualification);

        dbContext.WorkerCortexJobAvailability.Add(
            availability);

        dbContext.CortexTasks.Add(
            task);

        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();
    }

    private static async Task SetAvailabilityAsync(
    AtlantisDbContext dbContext,
    bool isAvailable,
    DateTimeOffset changedAt)
    {
        var availability =
            await dbContext.WorkerCortexJobAvailability
                .SingleAsync(
                    entity =>
                        entity
                            .WorkerCortexJobQualificationId ==
                        QualificationId);

        availability.IsAvailable =
            isAvailable;

        availability.ChangedAt =
            changedAt;

        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();
    }

    private static async Task SetDefinitionDeactivatedAtAsync(
        AtlantisDbContext dbContext,
        DateTimeOffset? deactivatedAt)
    {
        var definition =
            await dbContext.CortexJobDefinitions
                .SingleAsync(
                    entity =>
                        entity.Id == DefinitionId);

        definition.DeactivatedAt =
            deactivatedAt;

        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();
    }

    private static async Task<DateTimeOffset>
        GetEligibleTimeAsync(
            AtlantisDbContext dbContext)
    {
        var availableAt =
            await dbContext.CortexTasks
                .Where(entity =>
                    entity.Id == TaskId)
                .Select(entity =>
                    entity.AvailableAt)
                .SingleAsync();

        return availableAt.Value.AddMinutes(1);
    }
}