using Atlantis.Api.Development.Predictions.Remote;
using Microsoft.AspNetCore.Mvc;

namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation.WorkOrders;

[ApiController]
[Route(
    "api/production/simulate-citizen-pass/work-orders")]
public sealed class
    SimulateCitizenPassWorkOrderController
    : ControllerBase
{
    private readonly
        SimulateCitizenPassWorkOrderService
            _service;

    private readonly ILogger<
        SimulateCitizenPassWorkOrderController>
        _logger;

    public SimulateCitizenPassWorkOrderController(
        SimulateCitizenPassWorkOrderService service,
        ILogger<
            SimulateCitizenPassWorkOrderController> logger)
    {
        _service =
            service ??
            throw new ArgumentNullException(
                nameof(service));

        _logger =
            logger ??
            throw new ArgumentNullException(
                nameof(logger));
    }

    [HttpPost]
    public async Task<ActionResult<
        TrueWorldPredictionResponseDto>>
        CreateAsync(
            TrueWorldPredictionRequestDto request,
            CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(
            request);

        var result =
            await _service.FindOrCreateAsync(
                request,
                cancellationToken);

        _logger.LogInformation(
            "Production simulate-citizen-pass work order " +
            "{WorkOrderId}; Cortex task {CortexTaskId}; " +
            "external request {ExternalRequestId}; " +
            "created {Created}; waiting for completion.",
            result.WorkOrderId,
            result.CortexTaskId,
            request.RequestId,
            result.Created);

        var completion =
            await _service.WaitForCompletionAsync(
                result.WorkOrderId,
                cancellationToken);

        _logger.LogInformation(
            "Returning completed simulate-citizen-pass " +
            "work order {WorkOrderId}; worker " +
            "{WorkerCitizenId}.",
            result.WorkOrderId,
            completion.WorkerCitizenId);

        return Ok(
            new TrueWorldPredictionResponseDto
            {
                WorkerCitizenId =
                    completion.WorkerCitizenId,

                RecordedAt =
                    completion.RecordedAt,

                RecordedBreath =
                    completion.RecordedBreath
            });
    }
}