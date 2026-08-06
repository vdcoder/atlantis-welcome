using System.Text.Json;
using Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation.Contracts;
using Atlantis.Api.Development.Predictions.Contracts;

namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation.WorkOrders;

public sealed class
    SimulateCitizenPassRecordedBreathFactory
{
    private static readonly JsonSerializerOptions
        JsonOptions =
            new()
            {
                PropertyNameCaseInsensitive =
                    true
            };

    public RecordedCitizenBreathDto Create(
        string resultSerialized)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            resultSerialized);

        var completion =
            JsonSerializer.Deserialize<
                SimulateCitizenPassCompletionDto>(
                    resultSerialized,
                    JsonOptions)
            ?? throw new InvalidOperationException(
                "The simulate-citizen-pass result could " +
                "not be deserialized.");

        return new RecordedCitizenBreathDto
        {
            EmbodiedPredictions =
                completion.Predictions
                    .Select(MapPrediction)
                    .ToList(),

            CortexToolCalls =
                Array.Empty<
                    RecordedCortexToolCallDto>()
        };
    }

    private static RecordedPredictionDto MapPrediction(
        SimulationPredictionDto prediction)
    {
        ArgumentNullException.ThrowIfNull(
            prediction);

        return new RecordedPredictionDto
        {
            Type =
                prediction.Type,

            X =
                prediction.X,

            Z =
                prediction.Z,

            Text =
                prediction.Text,

            TargetQuery =
                prediction.TargetQuery,

            Direction =
                prediction.Direction is null
                    ? null
                    : new RecordedDirectionDto
                    {
                        X =
                            prediction.Direction.X,

                        Y =
                            prediction.Direction.Y,

                        Z =
                            prediction.Direction.Z
                    }
        };
    }
}