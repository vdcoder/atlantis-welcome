using System.Text.Json;
using Atlantis.Api.Citizens.Brain;
using Atlantis.Api.Common;
using Atlantis.Api.Development.Predictions.Contracts;

namespace Atlantis.Api.Development.Predictions.Serialization;

public sealed class RecordedCitizenBreathSerializer
{
    private static readonly JsonSerializerOptions
        JsonOptions =
            new()
            {
                PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase,

                PropertyNameCaseInsensitive =
                    true
            };

    public string Serialize(
        RecordedCitizenBreathDto breath)
    {
        ArgumentNullException.ThrowIfNull(
            breath);

        return JsonSerializer.Serialize(
            breath,
            JsonOptions);
    }

    public RecordedCitizenBreathDto Deserialize(
        string serialized)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            serialized);

        var breath =
            JsonSerializer.Deserialize<
                RecordedCitizenBreathDto>(
                    serialized,
                    JsonOptions)
            ?? throw new InvalidOperationException(
                "Recorded citizen breath could not be read.");

        if (breath.EmbodiedPredictions is null)
        {
            throw new InvalidOperationException(
                "Recorded citizen breath contains no " +
                "embodied-prediction collection.");
        }

        if (breath.CortexToolCalls is null)
        {
            throw new InvalidOperationException(
                "Recorded citizen breath contains no " +
                "Cortex tool-call collection.");
        }

        return breath;
    }

    public IReadOnlyList<Prediction>
    DeserializeEmbodiedPredictions(
        string serialized)
    {
        return ToEmbodiedPredictions(
            Deserialize(
                serialized));
    }

    public IReadOnlyList<Prediction>
        ToEmbodiedPredictions(
            RecordedCitizenBreathDto breath)
    {
        ArgumentNullException.ThrowIfNull(
            breath);

        if (breath.EmbodiedPredictions is null)
        {
            throw new InvalidOperationException(
                "Recorded citizen breath contains no " +
                "embodied-prediction collection.");
        }

        return breath.EmbodiedPredictions
            .Select(
                ToPrediction)
            .ToArray();
    }

    private static Prediction ToPrediction(
        RecordedPredictionDto prediction)
    {
        ArgumentNullException.ThrowIfNull(
            prediction);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            prediction.Type);

        return prediction.Type switch
        {
            RecordedPredictionTypes.Wait =>
                new WaitPrediction(),

            RecordedPredictionTypes.Move =>
                new MovePrediction(
                    RequireValue(
                        prediction.X,
                        prediction.Type,
                        "x"),

                    RequireValue(
                        prediction.Z,
                        prediction.Type,
                        "z")),

            RecordedPredictionTypes.Say =>
                new SayPrediction(
                    RequireText(
                        prediction.Text,
                        prediction.Type,
                        "text")),

            RecordedPredictionTypes.Touch =>
                new TouchPrediction(
                    RequireText(
                        prediction.TargetQuery,
                        prediction.Type,
                        "targetQuery"),

                    RequireText(
                        prediction.Text,
                        prediction.Type,
                        "text")),

            RecordedPredictionTypes.TurnTorso =>
                new TurnTorsoPrediction(
                    ToDirection(
                        prediction.Direction,
                        prediction.Type)),

            RecordedPredictionTypes.SetGaze =>
                new SetGazePrediction(
                    ToDirection(
                        prediction.Direction,
                        prediction.Type)),

            RecordedPredictionTypes.FaceAndLook =>
                new FaceAndLookPrediction(
                    ToDirection(
                        prediction.Direction,
                        prediction.Type)),

            _ =>
                throw new InvalidOperationException(
                    $"Unsupported recorded prediction type " +
                    $"'{prediction.Type}'.")
        };
    }

    private static Direction ToDirection(
        RecordedDirectionDto? direction,
        string predictionType)
    {
        if (direction is null)
        {
            throw new InvalidOperationException(
                $"Recorded prediction '{predictionType}' " +
                "requires a direction.");
        }

        if (!float.IsFinite(direction.X) ||
            !float.IsFinite(direction.Y) ||
            !float.IsFinite(direction.Z))
        {
            throw new InvalidOperationException(
                $"Recorded prediction '{predictionType}' " +
                "contains a non-finite direction.");
        }

        var value =
            new Direction(
                direction.X,
                direction.Y,
                direction.Z);

        if (value.LengthSquared <
            Direction.MinimumLengthSquared)
        {
            throw new InvalidOperationException(
                $"Recorded prediction '{predictionType}' " +
                "contains a zero direction.");
        }

        return value.Normalize();
    }

    private static float RequireValue(
        float? value,
        string predictionType,
        string propertyName)
    {
        if (!value.HasValue ||
            !float.IsFinite(value.Value))
        {
            throw new InvalidOperationException(
                $"Recorded prediction '{predictionType}' " +
                $"requires a finite '{propertyName}' value.");
        }

        return value.Value;
    }

    private static string RequireText(
        string? value,
        string predictionType,
        string propertyName)
    {
        if (string.IsNullOrWhiteSpace(
                value))
        {
            throw new InvalidOperationException(
                $"Recorded prediction '{predictionType}' " +
                $"requires '{propertyName}'.");
        }

        return value;
    }
}