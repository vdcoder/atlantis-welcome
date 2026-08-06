using System.Text.Json;
using Atlantis.Api.Citizens.Brain.CortexJobs.Context;
using Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation.Contracts;

namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation;

public sealed class
    SimulateCitizenPassDevelopmentResultFactory
{
    private const long VisibleProofSequenceNumber =
        20;

    private const string VisibleProofText =
        "Orestes completed Simba's twentieth recorded " +
        "breath through the true world.";

    private static readonly JsonSerializerOptions
        JsonOptions =
            new()
            {
                PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase
            };

    public string CreateSerializedResult(
        PrimedCortexTaskContext task)
    {
        ArgumentNullException.ThrowIfNull(
            task);

        var sequenceNumber =
            ReadSequenceNumber(
                task.InstructionsAndInputs);

        var prediction =
            sequenceNumber ==
                VisibleProofSequenceNumber
                ? CreateVisibleProofPrediction()
                : CreateWaitPrediction();

        var completion =
            new SimulateCitizenPassCompletionDto
            {
                Predictions =
                    [prediction]
            };

        return JsonSerializer.Serialize(
            completion,
            JsonOptions);
    }

    private static SimulationPredictionDto
        CreateVisibleProofPrediction()
    {
        return new SimulationPredictionDto
        {
            Type =
                SimulationPredictionTypes.Say,

            Text =
                VisibleProofText
        };
    }

    private static SimulationPredictionDto
        CreateWaitPrediction()
    {
        return new SimulationPredictionDto
        {
            Type =
                SimulationPredictionTypes.Wait
        };
    }

    private static long ReadSequenceNumber(
        string instructionsAndInputs)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            instructionsAndInputs);

        using var outerDocument =
            JsonDocument.Parse(
                instructionsAndInputs);

        var outerRoot =
            outerDocument.RootElement;

        var serializedInput =
            GetRequiredStringProperty(
                outerRoot,
                "PassContextAndCurrentPositionSerialized");

        using var inputDocument =
            JsonDocument.Parse(
                serializedInput);

        var inputRoot =
            inputDocument.RootElement;

        var sequenceProperty =
            GetRequiredProperty(
                inputRoot,
                "sequenceNumber");

        if (!sequenceProperty.TryGetInt64(
                out var sequenceNumber))
        {
            throw new InvalidOperationException(
                "The simulate-citizen-pass sequence number " +
                "was not a valid Int64 value.");
        }

        return sequenceNumber;
    }

    private static string GetRequiredStringProperty(
        JsonElement element,
        string propertyName)
    {
        var property =
            GetRequiredProperty(
                element,
                propertyName);

        if (property.ValueKind !=
            JsonValueKind.String)
        {
            throw new InvalidOperationException(
                $"Required property '{propertyName}' was " +
                "not a JSON string.");
        }

        return property.GetString()
            ?? throw new InvalidOperationException(
                $"Required property '{propertyName}' " +
                "contained a null value.");
    }

    private static JsonElement GetRequiredProperty(
        JsonElement element,
        string propertyName)
    {
        foreach (var property in
                 element.EnumerateObject())
        {
            if (string.Equals(
                    property.Name,
                    propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return property.Value;
            }
        }

        throw new InvalidOperationException(
            $"Required property '{propertyName}' was not " +
            "present in the simulate-citizen-pass task.");
    }
}