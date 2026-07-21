using Atlantis.Api.Citizens.AgentKernel;
using Atlantis.Api.Citizens.Brain;
using Atlantis.Api.Citizens.Brain.BrainTools;
using Atlantis.Api.Citizens.Interaction;
using Atlantis.Api.Citizens.Perception;
using Atlantis.Api.Citizens.Runtime;
using Atlantis.Api.Data;
using Atlantis.Api.Models;
using Atlantis.Api.World;
using Atlantis.Api.World.Actions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Atlantis")
    ?? "Host=postgres;Port=5432;Database=atlantis;Username=atlantis;Password=atlantis-dev-password";

builder.Services.AddDbContext<AtlantisDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register world infrastructure as singletons
var worldState = new WorldState();

builder.Services.AddSingleton(worldState);
builder.Services.AddSingleton<WorldPersistenceService>();
builder.Services.AddSingleton<WorldTransitionProcessor>();
builder.Services.AddSingleton<WorldActionProcessor>();
builder.Services.AddSingleton<WorldRuntime>();

// Register citizen agent infrastructure as singletons
builder.Services.AddSingleton<MoveTo>();
builder.Services.AddSingleton<Say>();
builder.Services.AddSingleton<Touch>();
builder.Services.AddSingleton<CitizenRuntime>();

builder.Services.AddSingleton<SemanticReach>();
builder.Services.AddSingleton<Predictor>();
builder.Services.AddSingleton<NearbyEntityPerception>();

builder.Services.AddHostedService<CitizenHostedService>();

var app = builder.Build();

// Initialize world state after build
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AtlantisDbContext>();
    await dbContext.Database.MigrateAsync();

    var registeredWorldState = scope.ServiceProvider.GetRequiredService<WorldState>();

    await InitializeWorldStateAsync(dbContext, registeredWorldState);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

static async Task InitializeWorldStateAsync(
    AtlantisDbContext dbContext,
    WorldState worldState)
{
    var persistedWorld = await dbContext.Worlds
        .Include(world => world.Entities)
        .FirstOrDefaultAsync(
            world => world.WorldId == "atlantis-welcome");

    if (false && persistedWorld != null)
    {
        var world = new Atlantis.Api.Models.World
        {
            WorldId = persistedWorld.WorldId,
            Time = persistedWorld.Time,
            Places =
            [
                new Place
                {
                    Id = "welcome-center",
                    Name = "Atlantis Welcome Center"
                }
            ],
            Entities = persistedWorld.Entities
                .Select(entity => new Entity
                {
                    Id = entity.EntityId,
                    Type = entity.Type,
                    Name = entity.Name,
                    PlaceId = entity.PlaceId,
                    Position = new Position(
                        entity.PositionX,
                        entity.PositionY,
                        entity.PositionZ),
                    CurrentUtterance =
                        entity.UtteranceText != null &&
                        entity.UtteranceSequence.HasValue &&
                        entity.UtteranceSpokenAt.HasValue
                            ? new Utterance
                            {
                                Sequence =
                                    entity.UtteranceSequence.Value,
                                Text =
                                    entity.UtteranceText,
                                SpokenAt =
                                    new DateTimeOffset(
                                        entity.UtteranceSpokenAt.Value,
                                        TimeSpan.Zero)
                            }
                            : null,
                    CurrentPrivateMessage =
                        entity.PrivateMessageText != null &&
                        entity.PrivateMessageSequence.HasValue &&
                        entity.PrivateMessageDeliveredAt.HasValue
                            ? new PrivateMessage
                            {
                                Sequence =
                                    entity.PrivateMessageSequence.Value,

                                SenderId =
                                    entity.PrivateMessageSenderId
                                    ?? string.Empty,

                                Text =
                                    entity.PrivateMessageText,

                                DeliveredAt =
                                    new DateTimeOffset(
                                        entity.PrivateMessageDeliveredAt.Value,
                                        TimeSpan.Zero)
        }
        : null
                })
                .ToList()
        };

        worldState.Initialize(world, persistedWorld.Revision);

        return;
    }

    var initialWorld = new Atlantis.Api.Models.World
    {
        WorldId = "atlantis-welcome",
        Time = DateTime.Parse(
            "2026-07-15T17:30:00Z",
            null,
            System.Globalization.DateTimeStyles.AdjustToUniversal),
        Places =
        [
            new Place
            {
                Id = "welcome-center",
                Name = "Atlantis Welcome Center"
            }
        ],
        Entities =
        [
            new Entity
            {
                Id = "orestes",
                Type = "citizen",
                Name = "Orestes",
                PlaceId = "welcome-center",
                Position = new Position(0, 0, 0)
            },
            new Entity
            {
                Id = "visitor-default",
                Type = "visitor",
                Name = "Visitor",
                PlaceId = "welcome-center",
                Position = new Position(0, 0, -1.5f)
            }
        ]
    };

    worldState.Initialize(initialWorld, 0);

    var persisted = new Atlantis.Api.Data.Entities.WorldEntity
    {
        Id = Guid.NewGuid(),
        WorldId = initialWorld.WorldId,
        Time = initialWorld.Time,
        Revision = 0,
        Entities = initialWorld.Entities
            .Select(entity =>
                new Atlantis.Api.Data.Entities.EntityEntity
                {
                    Id = Guid.NewGuid(),
                    EntityId = entity.Id,
                    Type = entity.Type,
                    Name = entity.Name,
                    PlaceId = entity.PlaceId,
                    PositionX = entity.Position.X,
                    PositionY = entity.Position.Y,
                    PositionZ = entity.Position.Z
                })
            .ToList()
    };

    dbContext.Worlds.Add(persisted);
    await dbContext.SaveChangesAsync();
}
