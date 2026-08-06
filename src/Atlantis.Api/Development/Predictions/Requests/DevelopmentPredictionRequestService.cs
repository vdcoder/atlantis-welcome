using System.Text.Json;
using Atlantis.Api.Data;
using Atlantis.Api.Development.Predictions.Domain;
using Atlantis.Api.Development.Predictions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Development.Predictions.Requests;

public sealed class DevelopmentPredictionRequestService
{
    private static readonly JsonSerializerOptions
        JsonOptions =
            new()
            {
                PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase
            };

    private readonly AtlantisDbContext
        _dbContext;

    public DevelopmentPredictionRequestService(
        AtlantisDbContext dbContext)
    {
        _dbContext =
            dbContext ??
            throw new ArgumentNullException(
                nameof(dbContext));
    }

    public async Task<bool> EnsureRequestedAsync(
        DevelopmentPredictionReplayKey key,
        DevelopmentPredictionRequestContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            key);

        ArgumentNullException.ThrowIfNull(
            context);

        ValidateKey(
            key);

        var exists =
            await _dbContext
                .DevelopmentPredictionRequests
                .AsNoTracking()
                .AnyAsync(
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

        if (exists)
        {
            return false;
        }

        _dbContext
            .DevelopmentPredictionRequests
            .Add(
                new DevelopmentPredictionRequestEntity
                {
                    Id =
                        Guid.NewGuid(),

                    DevelopmentWorldId =
                        key.DevelopmentWorldId,

                    ScenarioId =
                        key.ScenarioId,

                    SimulatedCitizenId =
                        key.SimulatedCitizenId,

                    SequenceNumber =
                        key.SequenceNumber,

                    ContextSerialized =
                        JsonSerializer.Serialize(
                            context,
                            JsonOptions),

                    ContextHash =
                        key.ContextHash,

                    Status =
                        (int)
                        DevelopmentPredictionRequestStatus
                            .Pending,

                    ContextContractVersion =
                        key.ContextContractVersion,

                    CreatedAt =
                        DateTimeOffset.UtcNow
                });

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return true;
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