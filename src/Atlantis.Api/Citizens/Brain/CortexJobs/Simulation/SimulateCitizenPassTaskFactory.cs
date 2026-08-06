using System.Text.Json;
using Atlantis.Api.Economy.Currency;
using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;
using Atlantis.Api.Citizens.Brain.CortexJobs.Tasks;
using Atlantis.Api.Citizens.Brain.CortexJobs.Simulation.Contracts;

namespace Atlantis.Api.Citizens.Brain.CortexJobs.Simulation;

public static class SimulateCitizenPassTaskFactory
{
    public static SimulateCitizenPassTaskCreation Create(
        SimulateCitizenPassRequest request,
        CortexJobDefinition cortexJobDefinition,
        decimal reward,
        DateTimeOffset now,
        Guid? desiredCortexTaskId = null)
    {
        Validate(
            request,
            cortexJobDefinition,
            reward);

        if (desiredCortexTaskId == Guid.Empty)
        {
            throw new ArgumentException(
                "Cortex task ID cannot be empty.",
                nameof(desiredCortexTaskId));
        }

        var cortexTaskId = desiredCortexTaskId ?? Guid.NewGuid();

        var instructions =
            JsonSerializer.Serialize(
                new SimulateCitizenPassInstructions
                {
                    CortexTaskType =
                        CortexTaskResultTypes
                            .SimulateCitizenPass,

                    ExternalScenarioId =
                        request.ExternalScenarioId,

                    PassContextAndCurrentPositionSerialized =
                        request
                            .PassContextAndCurrentPositionSerialized,

                    OutputType =
                        "IReadOnlyList<Prediction>"
                });

        var cortexTask = new CortexTask
        {
            Id = cortexTaskId,
            CortexJobDefinitionId = cortexJobDefinition.Id,
            Instructions = instructions,
            Reward = AtlantisCurrency.Normalize(reward),
            Currency = AtlantisCurrency.Code,
            CompletionInboxId =
                cortexJobDefinition.CompletionInboxId,
            FailureInboxId =
                cortexJobDefinition.FailureInboxId,
            Status = CortexTaskStatus.Available,
            CreatedAt = now,
            AvailableAt = now
        };

        var simulationTask =
            new SimulateCitizenPassTask
            {
                CortexTaskId = cortexTaskId,
                ExternalCitizenId =
                    request.ExternalCitizenId,
                ExternalWorldId =
                    request.ExternalWorldId,
                ExternalRequestId =
                    request.ExternalRequestId,
                ExternalScenarioId =
                    request.ExternalScenarioId,
                ReceivedAt = request.ReceivedAt
            };

        return new SimulateCitizenPassTaskCreation
        {
            CortexTask = cortexTask,
            SimulationTask = simulationTask
        };
    }

    private static void Validate(
        SimulateCitizenPassRequest request,
        CortexJobDefinition cortexJobDefinition,
        decimal reward)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(cortexJobDefinition);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.ExternalCitizenId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.ExternalWorldId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.ExternalRequestId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.ExternalScenarioId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request
                .PassContextAndCurrentPositionSerialized);

        if (!cortexJobDefinition.IsActiveAt(DateTimeOffset.UtcNow))
        {
            throw new InvalidOperationException(
                $"Cortex Job definition '{cortexJobDefinition.Id}' " +
                "is inactive.");
        }

        if (reward <= 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(reward),
                "Reward must be greater than zero.");
        }

        if (!AtlantisCurrency.IsValidAmount(reward))
        {
            throw new ArgumentException(
                $"Reward must contain no more than " +
                $"{AtlantisCurrency.DecimalPlaces} " +
                "decimal places.",
                nameof(reward));
        }
    }
}