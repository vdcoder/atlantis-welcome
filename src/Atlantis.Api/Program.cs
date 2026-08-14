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
using Atlantis.Api.World.Entities;
using Atlantis.Api.Development.Predictions;
using Atlantis.Api.Development.Predictions.Remote;
using Atlantis.Api.Development.Predictions.Requests;
using Atlantis.Api.Development.Predictions.Serialization;
using Atlantis.Api.Economy.Ledger;
using Atlantis.Api.Persistence;
using Atlantis.Api.Persistence.Records;
using Atlantis.Api.World;
using Atlantis.Api.World.Actions;
using Atlantis.Api.World.Authorization;
using Atlantis.Api.World.EmbodiedControl;
using Atlantis.Api.World.Orbs;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Atlantis.Api.Persistence.Seed;
using Atlantis.Api.World.Spatial;
using Atlantis.Api.Common;
using Atlantis.Api.Persistence.Services;

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

var databaseTarget =
    builder.Environment.IsEnvironment("Testing")
        ? "Testing"
        : "Development";

Console.WriteLine(
    $"Atlantis database target: " +
    $"{databaseTarget} - " +
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
builder.Services.AddSingleton<WorldStateLoader>();
builder.Services.AddSingleton<WorldRuntime>();

builder.Services.AddSingleton<OrbPositionResolver>();
builder.Services.AddSingleton<SensoryOrbPerception>();
builder.Services.AddSingleton<CortexTaskFeedbackService>();

// Register citizen agent infrastructure as singletons
builder.Services.AddSingleton<MoveTo>();
builder.Services.AddSingleton<Say>();
builder.Services.AddSingleton<Touch>();
builder.Services.AddSingleton<TurnTorso>();
builder.Services.AddSingleton<SetGaze>();
builder.Services.AddSingleton<FaceAndLook>();

builder.Services.AddScoped<CitizenRuntime>();
builder.Services.AddScoped<EmbodiedPredictionExecutor>();

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

builder.Services.AddSingleton<
    ISensoryPathResolver,
    AlwaysClearSensoryPathResolver>();

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

//if (productionVerifier.IsProduction)
{
    citizenControllerRegistry.Register<
        CitizenEmbodiedController>(
            "orestes");
}
//else
//{
//    citizenControllerRegistry.Register<
//        RecordedDevelopmentCitizenController>(
//            "orestes");
//}

// Initialize world state after build
using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<
                AtlantisDbContext>();

    await dbContext.Database.MigrateAsync();

    await VisualAttributeSeed.EnsureSeededAsync(
        dbContext);

    await VoiceAttributeSeed.EnsureSeededAsync(
        dbContext);

    var registeredWorldState =
        scope.ServiceProvider
            .GetRequiredService<
                WorldState>();

    var worldStateLoader =
        scope.ServiceProvider
            .GetRequiredService<WorldStateLoader>();

    await worldStateLoader.InitializeAsync(
        dbContext,
        registeredWorldState);

    dbContext.ChangeTracker.Clear();

    await EnsureKnownIdentityEntitiesAsync(
        dbContext,
        registeredWorldState);

    var sponsorshipService =
        scope.ServiceProvider
            .GetRequiredService<
                CitizenSponsorshipService>();

    await CitizenSponsorshipSeed.EnsureSeededAsync(
        dbContext,
        sponsorshipService);

    await AtlantisDatabaseSeed.EnsureSeededAsync(
        dbContext);

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

    var positionChangedAt =
        DateTimeOffset.UtcNow;

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

            PositionChangedAt =
                positionChangedAt,

            Embodiment =
                null
        };

    if (!persistedIdentityExists)
    {
        dbContext.Entities.Add(
            new EntityRecord
            {
                EntityId =
                    victor.Id,

                WorldId =
                    persistedWorldId,

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
                    victor.Position.Z,

                PositionChangedAt =
                    victor.PositionChangedAt.UtcDateTime,
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