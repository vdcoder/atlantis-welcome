using System.Text.Json;
using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;
using Atlantis.Api.Citizens.Brain.CortexJobs.Simulation;
using Atlantis.Api.Citizens.Brain.CortexJobs.Simulation.Contracts;
using Atlantis.Api.Data;
using Atlantis.Api.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Persistence;

public static class CortexTaskSeed
{
    private const string ExternalCitizenId =
        "simba-001";

    private const string ExternalWorldId =
        "development-world-001";

    private const string ExternalRequestId =
        "first-simulated-pass";

    private const string ExternalScenarioId =
        "welcome-center-baseline";

    private const decimal Reward =
        1.00m;

    public static async Task EnsureSeededAsync(
        AtlantisDbContext dbContext,
        CortexTaskCreationService creationService,
        CancellationToken cancellationToken = default)
    {
        var knownTaskId =
            KnownCortexTasks
                .FirstSimulateCitizenPassTaskId;

        var taskAlreadyExists =
            await dbContext.CortexTasks
                .AnyAsync(
                    entity =>
                        entity.Id == knownTaskId,
                    cancellationToken);

        if (taskAlreadyExists)
        {
            return;
        }

        var externalRequestAlreadyExists =
            await dbContext.SimulateCitizenPassTasks
                .AnyAsync(
                    entity =>
                        entity.ExternalWorldId ==
                            ExternalWorldId &&
                        entity.ExternalRequestId ==
                            ExternalRequestId,
                    cancellationToken);

        if (externalRequestAlreadyExists)
        {
            throw new InvalidOperationException(
                "The first simulation request already exists " +
                "under a different Cortex task ID.");
        }

        var definitionEntity =
            await dbContext.CortexJobDefinitions
                .Include(entity =>
                    entity.ScheduleConditions)
                .SingleAsync(
                    entity =>
                        entity.Id ==
                        KnownCortexJobDefinitions
                            .AtlantisDevelopmentSimulateCitizenPassId,
                    cancellationToken);

        var definition =
            CortexJobDefinitionMapper.ToDomain(
                definitionEntity);

        var now =
            DateTimeOffset.UtcNow;

        var contextAndPosition =
            JsonSerializer.Serialize(
                new
                {
                    context = new
                    {
                        worldId =
                            ExternalWorldId,

                        scenarioId =
                            ExternalScenarioId,

                        citizenId =
                            ExternalCitizenId,

                        observations = new[]
                        {
                            "A visitor is standing nearby " +
                            "in the Welcome Center."
                        }
                    },

                    currentPosition = new
                    {
                        x = 0,
                        y = 0,
                        z = 0
                    }
                });

        var request =
            new SimulateCitizenPassRequest
            {
                ExternalCitizenId =
                    ExternalCitizenId,

                ExternalWorldId =
                    ExternalWorldId,

                ExternalRequestId =
                    ExternalRequestId,

                ExternalScenarioId =
                    ExternalScenarioId,

                PassContextAndCurrentPositionSerialized =
                    contextAndPosition,

                ReceivedAt =
                    now
            };

        var creation =
            SimulateCitizenPassTaskFactory.Create(
                request,
                definition,
                Reward,
                now,
                knownTaskId);

        var created =
            await creationService
                .TryCreateSimulateCitizenPassTaskAsync(
                    creation,
                    cancellationToken);

        if (!created)
        {
            throw new InvalidOperationException(
                "The first simulation request appeared " +
                "while its seed was being created.");
        }
    }
}