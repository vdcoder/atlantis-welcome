using Atlantis.Api.Development.Predictions.Contracts;
using Atlantis.Api.Development.Predictions.Serialization;
using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Persistence.Seed;

public static class DevelopmentRecordedPredictionSeed
{
    public const string DevelopmentWorldId =
        "development-world-001";

    public const string ScenarioId =
        "recorded-replay-smoke-test";

    public const string SimulatedCitizenId =
        "simba-001";

    public const long SequenceNumber =
        1;

    public const int ContextContractVersion =
        1;

    public const string ContextHash =
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    public static async Task EnsureSeededAsync(
        AtlantisDbContext dbContext,
        RecordedCitizenBreathSerializer serializer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            dbContext);

        ArgumentNullException.ThrowIfNull(
            serializer);

        var recordedBreath1 =
            new RecordedCitizenBreathDto
            {
                EmbodiedPredictions =
                [
                    new RecordedPredictionDto
                    {
                        Type =
                            RecordedPredictionTypes.Say,

                        Text =
                            "Playing a recorded development breath."
                    },

                    new RecordedPredictionDto
                    {
                        Type =
                            RecordedPredictionTypes.FaceAndLook,

                        Direction =
                            new RecordedDirectionDto
                            {
                                X = 1f,
                                Y = 0f,
                                Z = 0f
                            }
                    }
                ],

                CortexToolCalls =
                    Array.Empty<
                        RecordedCortexToolCallDto>()
            };

        var recordedBreath2 =
            new RecordedCitizenBreathDto
            {
                EmbodiedPredictions =
                [
                    new RecordedPredictionDto
                    {
                        Type =
                            RecordedPredictionTypes.Say,

                        Text =
                            "Playing recorded development breath two."
                    },

                    new RecordedPredictionDto
                    {
                        Type =
                            RecordedPredictionTypes.FaceAndLook,

                        Direction =
                            new RecordedDirectionDto
                            {
                                X = 0f,
                                Y = 0f,
                                Z = -1f
                            }
                    }
                ],

                CortexToolCalls =
                    Array.Empty<
                        RecordedCortexToolCallDto>()
            };

        await EnsureRecordedBreathAsync(
            dbContext,
            serializer,
            SequenceNumber,
            recordedBreath1,
            cancellationToken);

        await EnsureRecordedBreathAsync(
            dbContext,
            serializer,
            SequenceNumber + 1,
            recordedBreath2,
            cancellationToken);
    }

    private static async Task EnsureRecordedBreathAsync(
        AtlantisDbContext dbContext,
        RecordedCitizenBreathSerializer serializer,
        long sequenceNumber,
        RecordedCitizenBreathDto recordedBreath,
        CancellationToken cancellationToken)
    {
        var exists =
            await dbContext
                .DevelopmentRecordedPredictions
                .AnyAsync(
                    entity =>
                        entity.DevelopmentWorldId ==
                            DevelopmentWorldId &&

                        entity.ScenarioId ==
                            ScenarioId &&

                        entity.SimulatedCitizenId ==
                            SimulatedCitizenId &&

                        entity.SequenceNumber ==
                            sequenceNumber &&

                        entity.ContextContractVersion ==
                            ContextContractVersion &&

                        entity.ContextHash ==
                            ContextHash,
                    cancellationToken);

        if (exists)
        {
            return;
        }

        dbContext.DevelopmentRecordedPredictions.Add(
            new DevelopmentRecordedPredictionRecord
            {
                Id =
                    Guid.NewGuid(),

                DevelopmentWorldId =
                    DevelopmentWorldId,

                ScenarioId =
                    ScenarioId,

                SimulatedCitizenId =
                    SimulatedCitizenId,

                SequenceNumber =
                    sequenceNumber,

                ContextContractVersion =
                    ContextContractVersion,

                ContextHash =
                    ContextHash,

                CognitiveOutputSerialized =
                    serializer.Serialize(
                        recordedBreath),

                WorkerCitizenId =
                    "development-seed",

                RecordedAt =
                    DateTimeOffset.UtcNow
            });

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}