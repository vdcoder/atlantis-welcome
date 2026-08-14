using System.Data;
using System.Text.Json;
using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;
using Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation.Contracts;
using Atlantis.Api.Development.Predictions.Remote;
using Atlantis.Api.Persistence;
using Atlantis.Api.Persistence.Records;
using Atlantis.Api.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation.WorkOrders;

public sealed class
    SimulateCitizenPassWorkOrderService
{
    private const decimal Reward =
        1.00m;

    private readonly AtlantisDbContext
        _dbContext;

    private readonly SimulateCitizenPassRecordedBreathFactory
        _recordedBreathFactory;

    public SimulateCitizenPassWorkOrderService(
        AtlantisDbContext dbContext,
        SimulateCitizenPassRecordedBreathFactory recordedBreathFactory)
    {
        _dbContext =
            dbContext ??
            throw new ArgumentNullException(
                nameof(dbContext));

        _recordedBreathFactory =
            recordedBreathFactory ??
            throw new ArgumentNullException(
                nameof(recordedBreathFactory));
    }

    public async Task<
        SimulateCitizenPassWorkOrderCreationResult>
        FindOrCreateAsync(
            TrueWorldPredictionRequestDto request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            request);

        ValidateRequest(
            request);

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

        try
        {
            var existing =
                await _dbContext
                    .SimulateCitizenPassWorkOrders
                    .SingleOrDefaultAsync(
                        entity =>
                            entity.ExternalRequestId ==
                                request.RequestId,
                        cancellationToken);

            if (existing is not null)
            {
                EnsureEquivalentRequest(
                    existing,
                    request);

                await transaction.CommitAsync(
                    cancellationToken);

                return new
                    SimulateCitizenPassWorkOrderCreationResult
                {
                    WorkOrderId =
                            existing.Id,

                    CortexTaskId =
                            existing.CortexTaskId,

                    Status =
                            (SimulateCitizenPassWorkOrderStatus)
                            existing.Status,

                    Created =
                            false
                };
            }

            var definitionEntity =
                await _dbContext
                    .CortexJobDefinitions
                    .Include(
                        entity =>
                            entity.ScheduleConditions)
                    .SingleAsync(
                        entity =>
                            entity.Id ==
                                KnownCortexJobDefinitions
                                    .AtlantisDevelopmentSimulateCitizenPassId,
                        cancellationToken);

            var definition =
                CortexJobDefinitionMapper.ToDomain(
                    definitionEntity);

            var now =
                DateTimeOffset.UtcNow;

            var cortexTaskId =
                Guid.NewGuid();

            var workOrderId =
                Guid.NewGuid();

            var simulationInputSerialized =
                JsonSerializer.Serialize(
                    new
                    {
                        operation =
                            "simulate_citizen_pass",

                        externalRequestId =
                            request.RequestId,

                        developmentWorldId =
                            request.DevelopmentWorldId,

                        scenarioId =
                            request.ScenarioId,

                        simulatedCitizenId =
                            request.SimulatedCitizenId,

                        sequenceNumber =
                            request.SequenceNumber,

                        contextContractVersion =
                            request.ContextContractVersion,

                        contextHash =
                            request.ContextHash,

                        contextSerialized =
                            request.ContextSerialized,

                        constraints =
                            new
                            {
                                simulationOnly =
                                    true,

                                workerIdentityIsNotSimulatedIdentity =
                                    true,

                                outputMustRemainInert =
                                    true
                            }
                    });

            var simulationRequest =
                new SimulateCitizenPassRequest
                {
                    ExternalCitizenId =
                        request.SimulatedCitizenId,

                    ExternalWorldId =
                        request.DevelopmentWorldId,

                    ExternalRequestId =
                        request.RequestId
                            .ToString("D"),

                    ExternalScenarioId =
                        request.ScenarioId,

                    PassContextAndCurrentPositionSerialized =
                        simulationInputSerialized,

                    ReceivedAt =
                        now
                };

            var creation =
                SimulateCitizenPassTaskFactory.Create(
                    simulationRequest,
                    definition,
                    Reward,
                    now,
                    cortexTaskId);

            var cortexTaskEntity =
                CortexTaskMapper.ToEntity(
                    creation.CortexTask);

            var simulationTaskEntity =
                SimulateCitizenPassTaskMapper.ToEntity(
                    creation.SimulationTask);

            var workOrderEntity =
                new SimulateCitizenPassWorkOrderRecord
                {
                    Id =
                        workOrderId,

                    CortexTaskId =
                        cortexTaskId,

                    ExternalRequestId =
                        request.RequestId,

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
                        request.ContextSerialized,

                    Status =
                        (int)
                        SimulateCitizenPassWorkOrderStatus
                            .Pending,

                    CreatedAt =
                        now,

                    CompletedAt =
                        null
                };

            cortexTaskEntity
                .SimulateCitizenPassTask =
                    simulationTaskEntity;

            simulationTaskEntity.WorkOrder =
                workOrderEntity;

            _dbContext.CortexTasks.Add(
                cortexTaskEntity);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return new
                SimulateCitizenPassWorkOrderCreationResult
            {
                WorkOrderId =
                        workOrderId,

                CortexTaskId =
                        cortexTaskId,

                Status =
                        SimulateCitizenPassWorkOrderStatus
                            .Pending,

                Created =
                        true
            };
        }
        catch
        {
            await transaction.RollbackAsync(
                CancellationToken.None);

            throw;
        }
    }

    public async Task<SimulateCitizenPassWorkOrderCompletion>
        WaitForCompletionAsync(
            Guid workOrderId,
            CancellationToken cancellationToken = default)
    {
        if (workOrderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Work order ID cannot be empty.",
                nameof(workOrderId));
        }

        var pollInterval =
            TimeSpan.FromMilliseconds(500);

        while (true)
        {
            cancellationToken
                .ThrowIfCancellationRequested();

            var completion =
                await (
                    from workOrder in
                        _dbContext
                            .SimulateCitizenPassWorkOrders
                            .AsNoTracking()

                    join assignment in
                        _dbContext
                            .CortexTaskAssignments
                            .AsNoTracking()

                        on workOrder.CortexTaskId
                        equals assignment.CortexTaskId

                    join result in
                        _dbContext
                            .CortexTaskResults
                            .AsNoTracking()

                        on assignment.Id
                        equals result
                            .CortexTaskAssignmentId

                    where
                        workOrder.Id ==
                            workOrderId &&

                        workOrder.Status ==
                            (int)
                            SimulateCitizenPassWorkOrderStatus
                                .Completed

                    select new
                    {
                        assignment.WorkerCitizenId,
                        result.CompletedAt,
                        result.ResultSerialized
                    })
                    .SingleOrDefaultAsync(
                        cancellationToken);

            if (completion is not null)
            {
                return new
                    SimulateCitizenPassWorkOrderCompletion
                {
                    WorkerCitizenId =
                            completion.WorkerCitizenId,

                    RecordedAt =
                            completion.CompletedAt,

                    RecordedBreath =
                            _recordedBreathFactory.Create(
                                completion.ResultSerialized)
                };
            }

            await Task.Delay(
                pollInterval,
                cancellationToken);
        }
    }

    private static void ValidateRequest(
        TrueWorldPredictionRequestDto request)
    {
        if (request.RequestId == Guid.Empty)
        {
            throw new ArgumentException(
                "Request ID cannot be empty.",
                nameof(request));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.DevelopmentWorldId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.ScenarioId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.SimulatedCitizenId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.ContextHash);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.ContextSerialized);

        if (request.SequenceNumber < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request),
                "Sequence number cannot be negative.");
        }

        if (request.ContextContractVersion <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request),
                "Context contract version must be positive.");
        }
    }

    private static void EnsureEquivalentRequest(
        SimulateCitizenPassWorkOrderRecord existing,
        TrueWorldPredictionRequestDto request)
    {
        var equivalent =
            existing.DevelopmentWorldId ==
                request.DevelopmentWorldId &&

            existing.ScenarioId ==
                request.ScenarioId &&

            existing.SimulatedCitizenId ==
                request.SimulatedCitizenId &&

            existing.SequenceNumber ==
                request.SequenceNumber &&

            existing.ContextContractVersion ==
                request.ContextContractVersion &&

            existing.ContextHash ==
                request.ContextHash &&

            existing.ContextSerialized ==
                request.ContextSerialized;

        if (!equivalent)
        {
            throw new InvalidOperationException(
                $"External request ID " +
                $"'{request.RequestId}' is already associated " +
                "with a different simulate-citizen-pass " +
                "work order payload.");
        }
    }
}