using Atlantis.Api.Development.Predictions.Remote;
using Atlantis.Api.Development.Predictions.Requests;
using Atlantis.Api.Development.Predictions.Serialization;
using Atlantis.Api.Persistence;
using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Development.Predictions;

public sealed class DevelopmentPredictionRequestProcessor
    : BackgroundService
{
    private readonly IServiceScopeFactory
        _scopeFactory;

    private readonly IConfiguration
        _configuration;

    private readonly ILogger<
        DevelopmentPredictionRequestProcessor>
        _logger;

    public DevelopmentPredictionRequestProcessor(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<
            DevelopmentPredictionRequestProcessor> logger)
    {
        _scopeFactory =
            scopeFactory ??
            throw new ArgumentNullException(
                nameof(scopeFactory));

        _configuration =
            configuration ??
            throw new ArgumentNullException(
                nameof(configuration));

        _logger =
            logger ??
            throw new ArgumentNullException(
                nameof(logger));
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var pollIntervalSeconds =
            _configuration.GetValue<int?>(
                "DevelopmentPredictionProcessor:" +
                "PollIntervalSeconds")
            ?? 5;

        var pollInterval =
            TimeSpan.FromSeconds(
                Math.Max(
                    1,
                    pollIntervalSeconds));

        _logger.LogInformation(
            "Development prediction request processor started.");

        // Allows the local echo endpoint to begin listening.
        await Task.Delay(
            TimeSpan.FromSeconds(1),
            stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var processed =
                await TryProcessOneAsync(
                    stoppingToken);

            if (!processed)
            {
                await Task.Delay(
                    pollInterval,
                    stoppingToken);
            }
        }
    }

    private async Task<bool> TryProcessOneAsync(
        CancellationToken cancellationToken)
    {
        await using var scope =
            _scopeFactory.CreateAsyncScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<
                    AtlantisDbContext>();

        var client =
            scope.ServiceProvider
                .GetRequiredService<
                    TrueWorldPredictionClient>();

        var serializer =
            scope.ServiceProvider
                .GetRequiredService<
                    RecordedCitizenBreathSerializer>();

        var request =
            await dbContext
                .DevelopmentPredictionRequests
                .Where(
                    entity =>
                        entity.Status ==
                            (int)
                            DevelopmentPredictionRequestStatus
                                .Pending)
                .OrderBy(
                    entity =>
                        entity.CreatedAt)
                .ThenBy(
                    entity =>
                        entity.Id)
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (request is null)
        {
            return false;
        }

        request.Status =
            (int)
            DevelopmentPredictionRequestStatus
                .Processing;

        await dbContext.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Processing development prediction request " +
            "{RequestId}; simulated citizen " +
            "{SimulatedCitizenId}; sequence " +
            "{SequenceNumber}.",
            request.Id,
            request.SimulatedCitizenId,
            request.SequenceNumber);

        var completion =
            await client.WaitForCompletionAsync(
                new TrueWorldPredictionRequestDto
                {
                    RequestId =
                        request.Id,

                    DevelopmentWorldId =
                        request.DevelopmentWorldId,

                    ScenarioId =
                        request.ScenarioId,

                    SimulatedCitizenId =
                        request.SimulatedCitizenId,

                    SequenceNumber =
                        request.SequenceNumber,

                    ContextContractVersion =
                        request.ContextContractVersion,

                    ContextHash =
                        request.ContextHash,

                    ContextSerialized =
                        request.ContextSerialized
                },
                cancellationToken);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            completion.WorkerCitizenId);

        ArgumentNullException.ThrowIfNull(
            completion.RecordedBreath);

        var existingRecording =
            await dbContext
                .DevelopmentRecordedPredictions
                .AnyAsync(
                    entity =>
                        entity.DevelopmentWorldId ==
                            request.DevelopmentWorldId &&

                        entity.ScenarioId ==
                            request.ScenarioId &&

                        entity.SimulatedCitizenId ==
                            request.SimulatedCitizenId &&

                        entity.SequenceNumber ==
                            request.SequenceNumber &&

                        entity.ContextContractVersion ==
                            request.ContextContractVersion &&

                        entity.ContextHash ==
                            request.ContextHash,
                    cancellationToken);

        if (!existingRecording)
        {
            dbContext
                .DevelopmentRecordedPredictions
                .Add(
                    new DevelopmentRecordedPredictionRecord
                    {
                        Id =
                            Guid.NewGuid(),

                        DevelopmentWorldId =
                            request.DevelopmentWorldId,

                        ScenarioId =
                            request.ScenarioId,

                        SimulatedCitizenId =
                            request.SimulatedCitizenId,

                        SequenceNumber =
                            request.SequenceNumber,

                        ContextContractVersion =
                            request.ContextContractVersion,

                        ContextHash =
                            request.ContextHash,

                        CognitiveOutputSerialized =
                            serializer.Serialize(
                                completion.RecordedBreath),

                        WorkerCitizenId =
                            completion.WorkerCitizenId,

                        RecordedAt =
                            completion.RecordedAt
                    });
        }

        request.Status =
            (int)
            DevelopmentPredictionRequestStatus
                .Completed;

        await dbContext.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Completed development prediction request " +
            "{RequestId}; recorded sequence " +
            "{SequenceNumber} from worker " +
            "{WorkerCitizenId}.",
            request.Id,
            request.SequenceNumber,
            completion.WorkerCitizenId);

        return true;
    }
}