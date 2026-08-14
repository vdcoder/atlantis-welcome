using Atlantis.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Atlantis.Api.IntegrationTests.Infrastructure;

public abstract class IsolatedDatabaseTest
    : IAsyncLifetime
{
    protected string ConnectionString =>
        _databaseManager.ActualConnectionString;

    private readonly TestDatabaseManager
        _databaseManager;

    protected IsolatedDatabaseTest()
    {
        _databaseManager =
            TestDatabaseEnvironment
                .CreateManager();
    }

    protected AtlantisDbContext
        CreateDbContext()
    {
        var options =
            new DbContextOptionsBuilder<
                AtlantisDbContext>()
                .UseNpgsql(
                    _databaseManager
                        .ActualConnectionString)
                .EnableDetailedErrors()
                .Options;

        return new AtlantisDbContext(
            options);
    }

    public async Task InitializeAsync()
    {
        await TestDatabaseRunCoordinator
            .EnsureTemplateSeededAsync(
                _databaseManager);

        await _databaseManager
            .RecreateActualDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await _databaseManager
            .DropActualDatabaseAsync();
    }
}