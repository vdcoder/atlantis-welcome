using System.Text.Json;
using Atlantis.Api.Citizens.Brain.CortexJobs
    .Completion;
using Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation.Contracts;
using Atlantis.Api.Common;

namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation;

public sealed class SimulateCitizenPassResultValidator
{
    private static readonly JsonSerializerOptions
        JsonOptions =
            new()
            {
                PropertyNameCaseInsensitive = true
            };

    public CortexTaskResultValidation Validate(
        string resultSerialized)
    {
        if (string.IsNullOrWhiteSpace(resultSerialized))
        {
            return CortexTaskResultValidation.Invalid(
                "The completion result cannot be empty.");
        }

        SimulateCitizenPassCompletionDto? completion;

        try
        {
            completion =
                JsonSerializer.Deserialize<
                    SimulateCitizenPassCompletionDto>(
                        resultSerialized,
                        JsonOptions);
        }
        catch (JsonException exception)
        {
            return CortexTaskResultValidation.Invalid(
                $"The completion result is not valid JSON: " +
                $"{exception.Message}");
        }

        if (completion is null)
        {
            return CortexTaskResultValidation.Invalid(
                "The completion result could not be read.");
        }

        if (completion.Predictions is null)
        {
            return CortexTaskResultValidation.Invalid(
                "The predictions collection is required.");
        }

        foreach (var prediction in completion.Predictions)
        {
            var validation =
                ValidatePrediction(prediction);

            if (!validation.IsValid)
            {
                return validation;
            }
        }

        return CortexTaskResultValidation.Valid();
    }

    private static CortexTaskResultValidation
        ValidatePrediction(
            SimulationPredictionDto prediction)
    {
        if (prediction is null)
        {
            return CortexTaskResultValidation.Invalid(
                "Predictions cannot contain null.");
        }

        if (string.IsNullOrWhiteSpace(prediction.Type))
        {
            return CortexTaskResultValidation.Invalid(
                "Each prediction requires a type.");
        }

        return prediction.Type switch
        {
            SimulationPredictionTypes.Wait =>
                CortexTaskResultValidation.Valid(),

            SimulationPredictionTypes.Move =>
                prediction.X.HasValue &&
                prediction.Z.HasValue
                    ? CortexTaskResultValidation.Valid()
                    : CortexTaskResultValidation.Invalid(
                        "A move prediction requires x and z."),

            SimulationPredictionTypes.Say =>
                !string.IsNullOrWhiteSpace(
                    prediction.Text)
                    ? CortexTaskResultValidation.Valid()
                    : CortexTaskResultValidation.Invalid(
                        "A say prediction requires text."),

            SimulationPredictionTypes.Touch =>
                !string.IsNullOrWhiteSpace(
                    prediction.TargetQuery) &&
                !string.IsNullOrWhiteSpace(
                    prediction.Text)
                    ? CortexTaskResultValidation.Valid()
                    : CortexTaskResultValidation.Invalid(
                        "A touch prediction requires " +
                        "targetQuery and text."),

            SimulationPredictionTypes.TurnTorso or
            SimulationPredictionTypes.SetGaze or
            SimulationPredictionTypes.FaceAndLook =>
                ValidateDirection(
                    prediction.Direction),

            _ =>
                CortexTaskResultValidation.Invalid(
                    $"Unsupported prediction type " +
                    $"'{prediction.Type}'.")
        };
    }

    private static CortexTaskResultValidation
        ValidateDirection(
            SimulationDirectionDto? direction)
    {
        if (direction is null)
        {
            return CortexTaskResultValidation.Invalid(
                "The prediction requires a direction.");
        }

        if (!float.IsFinite(direction.X) ||
            !float.IsFinite(direction.Y) ||
            !float.IsFinite(direction.Z))
        {
            return CortexTaskResultValidation.Invalid(
                "Direction components must be finite.");
        }

        var value =
            new Direction(
                direction.X,
                direction.Y,
                direction.Z);

        if (value.LengthSquared <
            Direction.MinimumLengthSquared)
        {
            return CortexTaskResultValidation.Invalid(
                "Direction cannot be zero.");
        }

        return CortexTaskResultValidation.Valid();
    }
}