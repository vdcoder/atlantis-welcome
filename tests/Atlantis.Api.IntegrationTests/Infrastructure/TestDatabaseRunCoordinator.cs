using Atlantis.Api.Persistence;
using Atlantis.Api.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.IntegrationTests.Infrastructure;

internal static class TestDatabaseRunCoordinator
{
    private static readonly SemaphoreSlim
        InitializationLock =
            new(1, 1);

    private static bool
        _templateSeeded;

    public static async Task EnsureTemplateSeededAsync(
        TestDatabaseManager manager,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            manager);

        if (_templateSeeded)
        {
            return;
        }

        await InitializationLock.WaitAsync(
            cancellationToken);

        try
        {
            if (_templateSeeded)
            {
                return;
            }

            await SeedTemplateAsync(
                manager.TemplateConnectionString,
                cancellationToken);

            Npgsql.NpgsqlConnection.ClearAllPools();

            _templateSeeded =
                true;
        }
        finally
        {
            InitializationLock.Release();
        }
    }

    private static async Task SeedTemplateAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        var options =
            new DbContextOptionsBuilder<
                AtlantisDbContext>()
                .UseNpgsql(connectionString)
                .EnableDetailedErrors()
                .Options;

        await using var dbContext =
            new AtlantisDbContext(
                options);

        await AtlantisDatabaseSeed
            .EnsureSeededAsync(
                dbContext,
                cancellationToken);
    }
}