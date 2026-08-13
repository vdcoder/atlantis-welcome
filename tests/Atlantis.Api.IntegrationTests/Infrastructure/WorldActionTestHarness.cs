using Atlantis.Api.Persistence;
using Atlantis.Api.Persistence.Services;
using Atlantis.Api.World;
using Atlantis.Api.World.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Atlantis.Api.IntegrationTests.Infrastructure;

public sealed class WorldActionTestHarness
    : IDisposable
{
    public ServiceProvider Services { get; }

    public WorldActionProcessor ActionProcessor { get; }

    public WorldActionTestHarness(
        string connectionString,
        WorldState worldState)
    {
        var services =
            new ServiceCollection();

        services.AddDbContext<AtlantisDbContext>(
            options =>
                options.UseNpgsql(
                    connectionString));

        Services =
            services.BuildServiceProvider();

        var persistenceService =
            new WorldPersistenceService(
                Services.GetRequiredService<
                    IServiceScopeFactory>());

        var transitionProcessor =
            new WorldTransitionProcessor(
                persistenceService);

        ActionProcessor =
            new WorldActionProcessor(
                worldState,
                transitionProcessor,
                NullLogger<
                    WorldActionProcessor>.Instance);
    }

    public void Dispose()
    {
        Services.Dispose();
    }
}