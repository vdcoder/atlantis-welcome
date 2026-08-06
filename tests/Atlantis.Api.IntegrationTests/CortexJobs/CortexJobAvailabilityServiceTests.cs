using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;
using Atlantis.Api.Citizens.Brain.CortexJobs.Tasks;
using Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;
using Atlantis.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.IntegrationTests.CortexJobs;

[CollectionDefinition(
    "Cortex database tests",
    DisableParallelization = true)]
public sealed class CortexJobAvailabilityServiceTests
{
    private static readonly Guid
        OrestesSimulateCitizenPassQualificationId =
            Guid.Parse(
                "d4767d3d-12b6-44ae-90ac-450f669fd201");

    [Fact]
    public async Task
        SetAvailabilityAsync_EnableFromFalse_ChangesState()
    {
        await using var dbContext =
            CreateDbContext();

        var service =
            new CortexJobAvailabilityService(
                dbContext);

        var qualificationId =
            OrestesSimulateCitizenPassQualificationId;

        var availability =
            await dbContext.WorkerCortexJobAvailability
                .SingleAsync(
                    entity =>
                        entity.WorkerCortexJobQualificationId ==
                        qualificationId);

        availability.IsAvailable = false;

        var baselineChangedAt =
            new DateTimeOffset(
                2026,
                7,
                23,
                15,
                0,
                0,
                TimeSpan.Zero);

        availability.ChangedAt =
            baselineChangedAt;

        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        var now =
            new DateTimeOffset(
                2026,
                7,
                23,
                16,
                0,
                0,
                TimeSpan.Zero);

        var result =
            await service.SetAvailabilityAsync(
                qualificationId,
                true,
                now);

        Assert.Equal(
            qualificationId,
            result.WorkerCortexJobQualificationId);

        Assert.True(result.IsAvailable);
        Assert.True(result.StateChanged);
        Assert.Equal(now, result.ChangedAt);

        dbContext.ChangeTracker.Clear();

        var persisted =
            await dbContext.WorkerCortexJobAvailability
                .AsNoTracking()
                .SingleAsync(
                    entity =>
                        entity.WorkerCortexJobQualificationId ==
                        qualificationId);

        Assert.True(persisted.IsAvailable);
        Assert.Equal(now, persisted.ChangedAt);

        var task =
            await dbContext.CortexTasks
                .AsNoTracking()
                .SingleAsync(
                    entity =>
                        entity.Id ==
                        KnownCortexTasks
                            .FirstSimulateCitizenPassTaskId);

        Assert.Equal(
            (int)CortexTaskStatus.Available,
            task.Status);

        Assert.False(
            await dbContext.CortexTaskAssignments
                .AnyAsync());
    }

    [Fact]
    public async Task
        SetAvailabilityAsync_WhenAlreadyEnabled_PreservesChangedAt()
    {
        await using var dbContext =
            CreateDbContext();

        var service =
            new CortexJobAvailabilityService(
                dbContext);

        var qualificationId =
            OrestesSimulateCitizenPassQualificationId;

        var originalChangedAt =
            new DateTimeOffset(
                2026,
                7,
                23,
                16,
                0,
                0,
                TimeSpan.Zero);

        var availability =
            await dbContext.WorkerCortexJobAvailability
                .SingleAsync(
                    entity =>
                        entity.WorkerCortexJobQualificationId ==
                        qualificationId);

        availability.IsAvailable = true;
        availability.ChangedAt =
            originalChangedAt;

        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        var later =
            originalChangedAt.AddHours(1);

        var result =
            await service.SetAvailabilityAsync(
                qualificationId,
                true,
                later);

        Assert.True(result.IsAvailable);
        Assert.False(result.StateChanged);

        Assert.Equal(
            originalChangedAt,
            result.ChangedAt);

        dbContext.ChangeTracker.Clear();

        var persisted =
            await dbContext.WorkerCortexJobAvailability
                .AsNoTracking()
                .SingleAsync(
                    entity =>
                        entity.WorkerCortexJobQualificationId ==
                        qualificationId);

        Assert.True(persisted.IsAvailable);

        Assert.Equal(
            originalChangedAt,
            persisted.ChangedAt);

        var task =
            await dbContext.CortexTasks
                .AsNoTracking()
                .SingleAsync(
                    entity =>
                        entity.Id ==
                        KnownCortexTasks
                            .FirstSimulateCitizenPassTaskId);

        Assert.Equal(
            (int)CortexTaskStatus.Available,
            task.Status);

        Assert.False(
            await dbContext.CortexTaskAssignments
                .AnyAsync());
    }

    private static AtlantisDbContext CreateDbContext()
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "ATLANTIS_TEST_CONNECTION")
            ?? "Host=localhost;Port=5432;" +
               "Database=atlantis;" +
               "Username=atlantis;" +
               "Password=atlantis-dev-password";

        var options =
            new DbContextOptionsBuilder<AtlantisDbContext>()
                .UseNpgsql(connectionString)
                .EnableDetailedErrors()
                .Options;

        return new AtlantisDbContext(options);
    }
}