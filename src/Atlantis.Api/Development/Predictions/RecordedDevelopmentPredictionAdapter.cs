using Atlantis.Api.Development.Predictions.Domain;
using Atlantis.Api.Development.Predictions.Serialization;
using Atlantis.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Development.Predictions;

public sealed class RecordedDevelopmentPredictionAdapter
{
    private readonly AtlantisDbContext
        _dbContext;

    private readonly RecordedCitizenBreathSerializer
        _serializer;

    public RecordedDevelopmentPredictionAdapter(
        AtlantisDbContext dbContext,
        RecordedCitizenBreathSerializer serializer)
    {
        _dbContext =
            dbContext ??
            throw new ArgumentNullException(
                nameof(dbContext));

        _serializer =
            serializer ??
            throw new ArgumentNullException(
                nameof(serializer));
    }

    public async Task<DevelopmentPredictionReplayResult>
        TryReplayAsync(
            DevelopmentPredictionReplayKey key,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            key);

        ValidateKey(
            key);

        var record =
            await _dbContext
                .DevelopmentRecordedPredictions
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    entity =>
                        entity.DevelopmentWorldId ==
                            key.DevelopmentWorldId &&

                        entity.ScenarioId ==
                            key.ScenarioId &&

                        entity.SimulatedCitizenId ==
                            key.SimulatedCitizenId &&

                        entity.SequenceNumber ==
                            key.SequenceNumber &&

                        entity.ContextContractVersion ==
                            key.ContextContractVersion &&

                        entity.ContextHash ==
                            key.ContextHash,
                    cancellationToken);

        if (record is null)
        {
            return DevelopmentPredictionReplayResult
                .Missing();
        }

        var recordedBreath =
            _serializer.Deserialize(
                record.CognitiveOutputSerialized);

        return DevelopmentPredictionReplayResult
            .Ready(
                record.Id,
                recordedBreath,
                record.WorkerCitizenId);
    }

    private static void ValidateKey(
        DevelopmentPredictionReplayKey key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            key.DevelopmentWorldId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            key.ScenarioId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            key.SimulatedCitizenId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            key.ContextHash);

        if (key.SequenceNumber < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(key),
                "Sequence number cannot be negative.");
        }

        if (key.ContextContractVersion <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(key),
                "Context contract version must be positive.");
        }
    }
}