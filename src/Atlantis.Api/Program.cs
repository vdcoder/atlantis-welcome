using Atlantis.Api.Citizens.Brain;
using Atlantis.Api.Citizens.Brain.CortexContext;
using Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;
using Atlantis.Api.Citizens.Brain.CortexJobs.Completion;
using Atlantis.Api.Citizens.Brain.CortexJobs.Context;
using Atlantis.Api.Citizens.Brain.CortexJobs.Feedback;
using Atlantis.Api.Citizens.Brain.CortexJobs.Remuneration;
using Atlantis.Api.Citizens.Brain.CortexJobs.Simulation;
using Atlantis.Api.Citizens.Brain.CortexJobs.Simulation.WorkOrders;
using Atlantis.Api.Citizens.Brain.CortexTools;
using Atlantis.Api.Citizens.Control;
using Atlantis.Api.Citizens.Control.EmbodiedTools;
using Atlantis.Api.Citizens.Interaction;
using Atlantis.Api.Citizens.Perception;
using Atlantis.Api.Citizens.Runtime;
using Atlantis.Api.Citizens.Sponsorship;
using Atlantis.Api.Data;
using Atlantis.Api.Data.Entities;
using Atlantis.Api.Development.Predictions;
using Atlantis.Api.Development.Predictions.Persistence;
using Atlantis.Api.Development.Predictions.Remote;
using Atlantis.Api.Development.Predictions.Requests;
using Atlantis.Api.Development.Predictions.Serialization;
using Atlantis.Api.Economy.Ledger;
using Atlantis.Api.Models;
using Atlantis.Api.Persistence;
using Atlantis.Api.World;
using Atlantis.Api.World.Actions;
using Atlantis.Api.World.Authorization;
using Atlantis.Api.World.EmbodiedControl;
using Atlantis.Api.World.Orbs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionStringName =
    builder.Environment.IsEnvironment("Testing")
        ? "AtlantisTest"
        : "Atlantis";

var connectionString =
    builder.Configuration.GetConnectionString(
        connectionStringName);

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        $"Connection string '{connectionStringName}' " +
        $"is not configured for environment " +
        $"'{builder.Environment.EnvironmentName}'.");
}

var connectionInfo = new NpgsqlConnectionStringBuilder(connectionString);

Console.WriteLine(
    $"Atlantis environment: {builder.Environment.EnvironmentName}");

Console.WriteLine(
    $"Atlantis database target: " +
    (builder.Environment.IsEnvironment("Testing") ? "Testing" : "Development") + " - " +
    $"{connectionInfo.Host}:" +
    $"{connectionInfo.Port}/" +
    $"{connectionInfo.Database}");

builder.Services.AddDbContext<AtlantisDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register world infrastructure as singletons
var worldState =
    new WorldState();

builder.Services.AddSingleton(
    worldState);

builder.Services.AddSingleton<WorldPersistenceService>();
builder.Services.AddSingleton<WorldTransitionProcessor>();
builder.Services.AddSingleton<WorldActionProcessor>();
builder.Services.AddSingleton<WorldRuntime>();

builder.Services.AddSingleton<WorldOrbCollection>();
builder.Services.AddSingleton<WorldOrbPositionResolver>();
builder.Services.AddSingleton<SensoryOrbPerception>();
builder.Services.AddSingleton<CortexTaskFeedbackOrbService>();

// Register citizen agent infrastructure as singletons
builder.Services.AddSingleton<MoveTo>();
builder.Services.AddSingleton<Say>();
builder.Services.AddSingleton<Touch>();
builder.Services.AddSingleton<TurnTorso>();
builder.Services.AddSingleton<SetGaze>();
builder.Services.AddSingleton<FaceAndLook>();

builder.Services.AddScoped<CitizenRuntime>();

builder.Services.AddSingleton<Predictor>();
builder.Services.AddSingleton<NearbyEntityPerception>();
builder.Services.AddSingleton<SemanticReach>();

builder.Services.AddScoped<LedgerService>();

builder.Services.AddHostedService<CitizenHostedService>();

builder.Services.AddScoped<CortexTaskCreationService>();
builder.Services.AddScoped<CortexJobAvailabilityService>();
builder.Services.AddScoped<CortexTaskPrimingService>();
builder.Services.AddScoped<IPrimedCortexTaskContextLoader, PrimedCortexTaskContextLoader>();

builder.Services.AddSingleton<EmbodiedControllerRegistry>(); // Dormant for now

builder.Services.AddScoped<CitizenCortexContextBuilder>();
builder.Services.AddScoped<CitizenEmbodiedController>();

builder.Services.AddScoped<SimulateCitizenPassResultValidator>();
builder.Services.AddScoped<CortexTaskCompletionService>();
builder.Services.AddHostedService<CortexTaskRemunerationProcessor>();
builder.Services.AddScoped<CompleteCortexTask>();

builder.Services.AddScoped<CitizenSponsorshipService>();

builder.Services.AddSingleton<CitizenControllerRegistry>();

builder.Services.AddScoped<CortexToolCallExecutor>();

builder.Services.AddSingleton<
    SimulateCitizenPassDevelopmentResultFactory>();

builder.Services.AddSingleton<
    SimulateCitizenPassRecordedBreathFactory>();

builder.Services.AddScoped<
    SimulateCitizenPassWorkOrderService>();

builder.Services.AddSingleton<
    RecordedCitizenBreathSerializer>();

builder.Services.AddSingleton<
    DevelopmentPredictionReplayCursor>();

builder.Services.AddScoped<
    RecordedDevelopmentPredictionAdapter>();

builder.Services.AddScoped<
    RecordedDevelopmentCitizenController>();

builder.Services
    .AddOptions<
        DevelopmentPredictionRequestOptions>()
    .Bind(
        builder.Configuration.GetSection(
            "DevelopmentPredictionRequests"))
    .Validate(
        options =>
            options.FirstSequenceNumber >= 1,
        "The first development prediction sequence " +
        "must be at least 1.")
    .Validate(
        options =>
            options.LastSequenceNumber >=
                options.FirstSequenceNumber,
        "The last development prediction sequence " +
        "cannot precede the first.")
    .ValidateOnStart();

builder.Services.AddSingleton<
    IDevelopmentPredictionRequestAuthorization,
    DevelopmentPredictionRequestAuthorization>();

builder.Services.AddScoped<
    DevelopmentPredictionRequestService>();

builder.Services
    .AddHttpClient<
        TrueWorldPredictionClient>(
        client =>
        {
            client.Timeout =
                Timeout.InfiniteTimeSpan;
        });

if (!builder.Environment.IsProduction())
{
    builder.Services.AddHostedService<
        DevelopmentPredictionRequestProcessor>();
}

builder.Services.AddSingleton<
    IProductionVerifier,
    ProductionVerifier>();

var app = builder.Build();

var productionVerifier =
    app.Services.GetRequiredService<
        IProductionVerifier>();

if (productionVerifier.IsProduction)
{
    await productionVerifier.VerifyBootAsync();
}

// orestes
var citizenControllerRegistry =
    app.Services.GetRequiredService<
        CitizenControllerRegistry>();

if (productionVerifier.IsProduction)
{
    citizenControllerRegistry.Register<
        CitizenEmbodiedController>(
            "orestes");
}
else
{
    citizenControllerRegistry.Register<
        RecordedDevelopmentCitizenController>(
            "orestes");
}

// Initialize world state after build
using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<
                AtlantisDbContext>();

    await dbContext.Database.MigrateAsync();

    var registeredWorldState =
        scope.ServiceProvider
            .GetRequiredService<
                WorldState>();

    await InitializeWorldStateAsync(
        dbContext,
        registeredWorldState);

    dbContext.ChangeTracker.Clear();

    await EnsureKnownIdentityEntitiesAsync(
        dbContext,
        registeredWorldState);

    await EconomySeed.EnsureSeededAsync(
        dbContext);

    var sponsorshipService =
        scope.ServiceProvider
            .GetRequiredService<
                CitizenSponsorshipService>();

    await CitizenSponsorshipSeed.EnsureSeededAsync(
        dbContext,
        sponsorshipService);

    await CortexJobDefinitionSeed.EnsureSeededAsync(
        dbContext);

    await CortexJobEmploymentSeed.EnsureSeededAsync(
        dbContext);

    var cortexTaskCreationService =
        scope.ServiceProvider
            .GetRequiredService<
                CortexTaskCreationService>();

    await CortexTaskSeed.EnsureSeededAsync(
        dbContext,
        cortexTaskCreationService);

    if (!productionVerifier.IsProduction)
    {
        var recordedBreathSerializer =
            scope.ServiceProvider
                .GetRequiredService<
                    RecordedCitizenBreathSerializer>();

        await DevelopmentRecordedPredictionSeed
            .EnsureSeededAsync(
                dbContext,
                recordedBreathSerializer);
    }
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

    if (persistedWorld != null)
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
                },
                new Place
                {
                    Id = "external",
                    Name = "Outside Atlantis"
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

                    Embodiment = MapEmbodiment(entity),

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
                            : null,
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
            },
            new Place
            {
                Id = "external",
                Name = "Outside Atlantis"
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
                Position = new Position(0f, 0f, 0f),

                Embodiment = new Embodiment
                {
                    TorsoFront = Direction.UnitZ,
                    GazeDirection = Direction.UnitZ
                }
            },
            new Entity
            {
                Id = "visitor-default",
                Type = "visitor",
                Name = "Visitor",
                PlaceId = "welcome-center",
                Position = new Position(0f, 0f, -1.5f),

                Embodiment = new Embodiment
                {
                    TorsoFront = Direction.UnitZ,
                    GazeDirection = Direction.UnitZ
                }
            },
            new Entity
            {
                Id = "human:victor",
                Type = "human",
                Name = "Victor",
                PlaceId = "external",
                Position = new Position(
                    0f,
                    0f,
                    0f),
                Embodiment = null
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
                    PositionZ = entity.Position.Z,
                    TorsoFrontX = entity.Embodiment?.TorsoFront.X,
                    TorsoFrontY = entity.Embodiment?.TorsoFront.Y,
                    TorsoFrontZ = entity.Embodiment?.TorsoFront.Z,
                    GazeDirectionX = entity.Embodiment?.GazeDirection.X,
                    GazeDirectionY = entity.Embodiment?.GazeDirection.Y,
                    GazeDirectionZ = entity.Embodiment?.GazeDirection.Z
                })
            .ToList()
    };

    dbContext.Worlds.Add(persisted);
    await dbContext.SaveChangesAsync();
}

static Embodiment? MapEmbodiment(EntityEntity entity)
{
    var hasNoOrientation =
        entity.TorsoFrontX is null &&
        entity.TorsoFrontY is null &&
        entity.TorsoFrontZ is null &&
        entity.GazeDirectionX is null &&
        entity.GazeDirectionY is null &&
        entity.GazeDirectionZ is null;

    if (hasNoOrientation)
    {
        return null;
    }

    var hasCompleteOrientation =
        entity.TorsoFrontX is not null &&
        entity.TorsoFrontY is not null &&
        entity.TorsoFrontZ is not null &&
        entity.GazeDirectionX is not null &&
        entity.GazeDirectionY is not null &&
        entity.GazeDirectionZ is not null;

    if (!hasCompleteOrientation)
    {
        throw new InvalidOperationException(
            $"Entity '{entity.EntityId}' contains partial " +
            "embodiment orientation data.");
    }

    return new Embodiment
    {
        TorsoFront =
            new Direction(
                entity.TorsoFrontX!.Value,
                entity.TorsoFrontY!.Value,
                entity.TorsoFrontZ!.Value)
            .Normalize(),

        GazeDirection =
            new Direction(
                entity.GazeDirectionX!.Value,
                entity.GazeDirectionY!.Value,
                entity.GazeDirectionZ!.Value)
            .Normalize()
    };
}

static async Task EnsureKnownIdentityEntitiesAsync(
    AtlantisDbContext dbContext,
    WorldState worldState,
    CancellationToken cancellationToken = default)
{
    ArgumentNullException.ThrowIfNull(
        dbContext);

    ArgumentNullException.ThrowIfNull(
        worldState);

    const string victorId =
        "human:victor";

    // The runtime may already contain Victor because this is
    // a fresh world or because he was loaded from persistence.
    if (worldState.World.Entities.Any(
            entity =>
                entity.Id == victorId))
    {
        return;
    }

    var persistedWorldId =
        await dbContext.Worlds
            .AsNoTracking()
            .Where(world =>
                world.WorldId ==
                    worldState.World.WorldId)
            .Select(world =>
                world.Id)
            .SingleAsync(
                cancellationToken);

    var persistedIdentityExists =
        await dbContext.Entities
            .AsNoTracking()
            .AnyAsync(
                entity =>
                    entity.WorldId ==
                        persistedWorldId &&
                    entity.EntityId ==
                        victorId,
                cancellationToken);

    var victor =
        new Entity
        {
            Id =
                victorId,

            Type =
                "human",

            Name =
                "Victor",

            PlaceId =
                "external",

            Position =
                new Position(
                    0f,
                    0f,
                    0f),

            Embodiment =
                null
        };

    if (!persistedIdentityExists)
    {
        dbContext.Entities.Add(
            new EntityEntity
            {
                Id =
                    Guid.NewGuid(),

                WorldId =
                    persistedWorldId,

                EntityId =
                    victor.Id,

                Type =
                    victor.Type,

                Name =
                    victor.Name,

                PlaceId =
                    victor.PlaceId,

                PositionX =
                    victor.Position.X,

                PositionY =
                    victor.Position.Y,

                PositionZ =
                    victor.Position.Z
            });

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    // Startup identity reconciliation.
    // This is not a live world event and intentionally does
    // not advance the world's revision.
    worldState.World.Entities.Add(
        victor);
}
