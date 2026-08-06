using Atlantis.Api.Development.Predictions.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Atlantis.Api.Development.Predictions.Remote;

[ApiController]
[Route(
    "api/development/prediction-request-echo")]
public sealed class
    DevelopmentPredictionRequestEchoController
    : ControllerBase
{
    private readonly ILogger<
        DevelopmentPredictionRequestEchoController>
        _logger;

    public DevelopmentPredictionRequestEchoController(
        ILogger<
            DevelopmentPredictionRequestEchoController>
            logger)
    {
        _logger =
            logger ??
            throw new ArgumentNullException(
                nameof(logger));
    }

    [HttpPost]
    public ActionResult<
        TrueWorldPredictionResponseDto>
        Complete(
            TrueWorldPredictionRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(
            request);

        _logger.LogInformation(
            "Echo true-world endpoint received request " +
            "{RequestId}; world {DevelopmentWorldId}; " +
            "scenario {ScenarioId}; citizen " +
            "{SimulatedCitizenId}; sequence " +
            "{SequenceNumber}; context version " +
            "{ContextContractVersion}; context hash " +
            "{ContextHash}; serialized context " +
            "{ContextSerialized}.",
            request.RequestId,
            request.DevelopmentWorldId,
            request.ScenarioId,
            request.SimulatedCitizenId,
            request.SequenceNumber,
            request.ContextContractVersion,
            request.ContextHash,
            request.ContextSerialized);

        return Ok(
            new TrueWorldPredictionResponseDto
            {
                WorkerCitizenId =
                    "echo-worker",

                RecordedAt =
                    DateTimeOffset.UtcNow,

                RecordedBreath =
                    new RecordedCitizenBreathDto
                    {
                        EmbodiedPredictions =
                        [
                            new RecordedPredictionDto
                            {
                                Type =
                                    RecordedPredictionTypes
                                        .Say,

                                Text =
                                    $"Echo completed recorded " +
                                    $"breath " +
                                    $"{request.SequenceNumber}."
                            }
                        ],

                        CortexToolCalls =
                            Array.Empty<
                                RecordedCortexToolCallDto>()
                    }
            });
    }
}