namespace Atlantis.Api.Persistence.Seed;

public static class AtlantisDatabaseSeed
{
    public static async Task EnsureSeededAsync(
        AtlantisDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            dbContext);

        await EconomySeed.EnsureSeededAsync(
            dbContext,
            cancellationToken);

        await CortexJobDefinitionSeed
            .EnsureSeededAsync(
                dbContext,
                cancellationToken);

        await CortexJobEmploymentSeed
            .EnsureSeededAsync(
                dbContext,
                cancellationToken);

        var taskCreationService =
            new CortexTaskCreationService(
                dbContext);

        await CortexTaskSeed.EnsureSeededAsync(
            dbContext,
            taskCreationService,
            cancellationToken);
    }
}