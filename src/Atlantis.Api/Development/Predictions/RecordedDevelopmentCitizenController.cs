using Atlantis.Api.Citizens.Brain;
using Atlantis.Api.Citizens.Brain.CortexTools;
using Atlantis.Api.Citizens.Control;
using Atlantis.Api.Development.Predictions.Domain;
using Atlantis.Api.Development.Predictions.Requests;
using Atlantis.Api.Development.Predictions.Serialization;
using Atlantis.Api.Persistence.Seed;
using Atlantis.Api.World.EmbodiedControl;
using Microsoft.Extensions.Options;

namespace Atlantis.Api.Development.Predictions;

public sealed class RecordedDevelopmentCitizenController
    : ICitizenController
{
    private readonly RecordedDevelopmentPredictionAdapter
        _adapter;

    private readonly RecordedCitizenBreathSerializer
        _serializer;

    private readonly DevelopmentPredictionReplayCursor
        _cursor;

    private readonly
        IDevelopmentPredictionRequestAuthorization
            _requestAuthorization;

    private readonly DevelopmentPredictionRequestService
        _requestService;

    private readonly DevelopmentPredictionRequestOptions
        _options;

    private readonly ILogger<
        RecordedDevelopmentCitizenController>
        _logger;

    public RecordedDevelopmentCitizenController(
        RecordedDevelopmentPredictionAdapter adapter,
        RecordedCitizenBreathSerializer serializer,
        DevelopmentPredictionReplayCursor cursor,
        IDevelopmentPredictionRequestAuthorization requestAuthorization,
        DevelopmentPredictionRequestService requestService,
        IOptions<DevelopmentPredictionRequestOptions> options,
        ILogger<RecordedDevelopmentCitizenController> logger)
    {
        _adapter =
            adapter ??
            throw new ArgumentNullException(
                nameof(adapter));

        _serializer =
            serializer ??
            throw new ArgumentNullException(
                nameof(serializer));

        _cursor =
            cursor ??
            throw new ArgumentNullException(
                nameof(cursor));

        _requestAuthorization =
            requestAuthorization ??
            throw new ArgumentNullException(
                nameof(requestAuthorization));

        _requestService =
            requestService ??
            throw new ArgumentNullException(
                nameof(requestService));

        _options =
            options?.Value ??
            throw new ArgumentNullException(
                nameof(options));

        _logger =
            logger ??
            throw new ArgumentNullException(
                nameof(logger));
    }

    public async Task<CitizenBreath>
        ProduceCitizenBreathAsync(
            EmbodiedControllerContext context,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            context);

        var sequenceNumber =
            _cursor.GetNextSequenceNumber();

        var replayKey =
            new DevelopmentPredictionReplayKey
            {
                DevelopmentWorldId =
                    DevelopmentRecordedPredictionSeed
                        .DevelopmentWorldId,

                ScenarioId =
                    DevelopmentRecordedPredictionSeed
                        .ScenarioId,

                SimulatedCitizenId =
                    DevelopmentRecordedPredictionSeed
                        .SimulatedCitizenId,

                SequenceNumber =
                    sequenceNumber,

                ContextContractVersion =
                    DevelopmentRecordedPredictionSeed
                        .ContextContractVersion,

                ContextHash =
                    DevelopmentRecordedPredictionSeed
                        .ContextHash
            };

        var replay =
            await _adapter.TryReplayAsync(
                replayKey,
                cancellationToken);

        if (replay.Status ==
            DevelopmentPredictionReplayStatus.Missing)
        {
            if (replayKey.SequenceNumber <
                    _options.FirstSequenceNumber ||
                replayKey.SequenceNumber >
                    _options.LastSequenceNumber)
            {
                _logger.LogInformation(
                    "Development replay sequence {SequenceNumber} " +
                    "is outside the configured automatic range " +
                    "{FirstSequenceNumber}-{LastSequenceNumber}.",
                    replayKey.SequenceNumber,
                    _options.FirstSequenceNumber,
                    _options.LastSequenceNumber);

                return CreateEmptyBreath(
                    context);
            }

            if (_requestAuthorization.IsAuthorized(
                    replayKey.DevelopmentWorldId,
                    replayKey.ScenarioId,
                    replayKey.SimulatedCitizenId))
            {
                var created =
                    await _requestService
                        .EnsureRequestedAsync(
                            replayKey,
                            new DevelopmentPredictionRequestContext
                            {
                                BodyEntityId =
                                    context.Entity.Id,

                                ObservedWorldRevision =
                                    context.ObservedWorldRevision,

                                ObservedAt =
                                    context.ObservedAt
                            },
                            cancellationToken);

                if (created)
                {
                    _logger.LogInformation(
                        "Created development prediction request " +
                        "for simulated citizen " +
                        "{SimulatedCitizenId}; world " +
                        "{DevelopmentWorldId}; scenario " +
                        "{ScenarioId}; sequence " +
                        "{SequenceNumber}.",
                        replayKey.SimulatedCitizenId,
                        replayKey.DevelopmentWorldId,
                        replayKey.ScenarioId,
                        replayKey.SequenceNumber);
                }
            }

            return CreateEmptyBreath(
                context);
        }

        if (replay.Status !=
                DevelopmentPredictionReplayStatus.Ready ||
            replay.RecordedBreath is null)
        {
            throw new InvalidOperationException(
                "Development prediction replay returned an " +
                "inconsistent result.");
        }

        var embodiedPredictions =
            _serializer.ToEmbodiedPredictions(
                replay.RecordedBreath);

        _cursor.Advance(
            sequenceNumber);

        _logger.LogInformation(
            "Replaying recorded development breath " +
            "{RecordedPredictionId} for entity {EntityId}; " +
            "simulated citizen {SimulatedCitizenId}; " +
            "sequence {SequenceNumber}; " +
            "recorded worker {WorkerCitizenId}.",
            replay.RecordedPredictionId,
            context.Entity.Id,
            DevelopmentRecordedPredictionSeed
                .SimulatedCitizenId,
            sequenceNumber,
            replay.WorkerCitizenId);

        return new CitizenBreath
        {
            EmbodiedPredictions =
                new PredictionBatch
                {
                    EntityId =
                        context.Entity.Id,

                    BasedOnWorldRevision =
                        context.ObservedWorldRevision,

                    ProducedAt =
                        DateTimeOffset.UtcNow,

                    Predictions =
                        embodiedPredictions
                },

            // Recorded Cortex calls remain inert.
            CortexToolCalls =
                Array.Empty<CortexToolCall>()
        };
    }

    private static CitizenBreath CreateEmptyBreath(
        EmbodiedControllerContext context)
    {
        return new CitizenBreath
        {
            EmbodiedPredictions =
                new PredictionBatch
                {
                    EntityId =
                        context.Entity.Id,

                    BasedOnWorldRevision =
                        context.ObservedWorldRevision,

                    ProducedAt =
                        DateTimeOffset.UtcNow,

                    Predictions =
                        Array.Empty<Prediction>()
                },

            CortexToolCalls =
                Array.Empty<CortexToolCall>()
        };
    }
}