using Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;
using Atlantis.Api.Citizens.Brain.CortexJobs.Context;
using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;
using Atlantis.Api.Citizens.Control;
using Atlantis.Api.Citizens.Perception;
using Atlantis.Api.World;
using Atlantis.Api.World.EmbodiedControl;

namespace Atlantis.Api.Citizens.Runtime
{
    public sealed class CitizenRuntime
    {
        private readonly WorldRuntime
        _worldRuntime;

        private readonly NearbyEntityPerception
            _perception;

        private readonly CitizenControllerRegistry
            _controllerRegistry;

        private readonly CortexToolCallExecutor
            _cortexToolCallExecutor;

        private readonly CortexTaskPrimingService
            _cortexTaskPrimingService;

        private readonly IPrimedCortexTaskContextLoader
            _primedCortexTaskContextLoader;

        private readonly SensoryOrbPerception
            _sensoryOrbPerception;

        private readonly IServiceProvider
            _serviceProvider;

        private readonly EmbodiedPredictionExecutor
            _predictionExecutor;

        private readonly ILogger<CitizenRuntime>
            _logger;

        public CitizenRuntime(
            WorldRuntime worldRuntime,
            NearbyEntityPerception perception,
            CitizenControllerRegistry controllerRegistry,
            CortexToolCallExecutor cortexToolCallExecutor,
            CortexTaskPrimingService cortexTaskPrimingService,
            IPrimedCortexTaskContextLoader primedCortexTaskContextLoader,
            SensoryOrbPerception sensoryOrbPerception,
            IServiceProvider serviceProvider,
            EmbodiedPredictionExecutor predictionExecutor,
            ILogger<CitizenRuntime> logger)
        {
            _worldRuntime =
                worldRuntime ??
                throw new ArgumentNullException(
                    nameof(worldRuntime));

            _perception =
                perception ??
                throw new ArgumentNullException(
                    nameof(perception));

            _controllerRegistry =
                controllerRegistry ??
                throw new ArgumentNullException(
                    nameof(controllerRegistry));

            _cortexToolCallExecutor =
                cortexToolCallExecutor ??
                throw new ArgumentNullException(
                    nameof(cortexToolCallExecutor));

            _cortexTaskPrimingService =
                cortexTaskPrimingService ??
                throw new ArgumentNullException(
                    nameof(cortexTaskPrimingService));

            _primedCortexTaskContextLoader =
                primedCortexTaskContextLoader ??
                throw new ArgumentNullException(
                    nameof(primedCortexTaskContextLoader));

            _sensoryOrbPerception =
                sensoryOrbPerception ??
                throw new ArgumentNullException(
                    nameof(sensoryOrbPerception));

            _serviceProvider =
                serviceProvider ??
                throw new ArgumentNullException(
                    nameof(serviceProvider));

            _predictionExecutor =
                predictionExecutor ??
                throw new ArgumentNullException(
                    nameof(predictionExecutor));

            _logger =
                logger ??
                throw new ArgumentNullException(
                    nameof(logger));
        }

        public async Task RunOneIterationAsync(
            string citizenId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                citizenId);

            var snapshot =
                _worldRuntime.GetSnapshot();

            var citizen =
                snapshot.World.Entities
                    .FirstOrDefault(
                        entity =>
                            entity.Id == citizenId);

            if (citizen is null)
            {
                _logger.LogWarning(
                    "\x1b[35mCitizen\x1b[0m {CitizenId} was not found.",
                    citizenId);

                return;
            }

            if (citizen.Embodiment is null)
            {
                _logger.LogWarning(
                    "\x1b[35mCitizen\x1b[0m {CitizenId} is not embodied.",
                    citizenId);

                return;
            }

            var observedAt =
                DateTimeOffset.UtcNow;

            await EnsureAtMostOnePrimedCortexTaskAsync(
                citizen.Id,
                observedAt,
                cancellationToken);

            var controller =
                _controllerRegistry.ResolveRequired(
                    citizen.Id,
                    _serviceProvider);

            var nearbyEntityBindings =
                _perception.Perceive(
                    citizen,
                    snapshot.World,
                    observedAt);

            var nearbyEntities =
                nearbyEntityBindings
                    .Select(
                        binding =>
                            binding.TransparentEntity)
                    .ToList();

            var auditoryEventBindings =
                _sensoryOrbPerception.Perceive(
                    citizen,
                    snapshot.World,
                    observedAt);

            var auditoryEvents =
                auditoryEventBindings
                    .Select(
                        binding =>
                            binding.TransparentAuditoryEvent)
                    .ToList();

            var controllerContext =
                new EmbodiedControllerContext
                {
                    Entity =
                        citizen,

                    ObservedWorldRevision =
                        snapshot.Revision,

                    ObservedAt =
                        observedAt,

                    NearbyEntities =
                        nearbyEntities,

                    AuditoryEvents =
                        auditoryEvents,
                };

            var breath =
                await controller.ProduceCitizenBreathAsync(
                    controllerContext,
                    cancellationToken);

            _logger.LogInformation(
                "Citizen {CitizenId} observing at {ObservedAt:O}. " +
                "Snapshot has {OrbCount} orb(s).",
                citizen.Id,
                observedAt,
                snapshot.World.Orbs.Count);

            //foreach (var orb in snapshot.World.Orbs)
            //{
            //    _logger.LogInformation(
            //        "Orb {OrbId}: source={SourceEntityId}, " +
            //        "created={CreatedAt:O}, expires={ExpiresAt:O}, " +
            //        "active={IsActive}",
            //        orb.Id,
            //        orb.SourceEntityId,
            //        orb.CreatedAt,
            //        orb.ExpiresAt,
            //        orb.IsActiveAt(observedAt));
            //}

            //foreach (var orb in sensoryOrbs)
            //{
            //    _logger.LogInformation(
            //        "PERCEIVED: Citizen {CitizenId} perceives {Modality} " +
            //        "from {SourceEntityId} at {Distance:F2}m: {Content}",
            //        citizen.Id,
            //        orb.Modality,
            //        orb.SourceEntityId,
            //        orb.Distance,
            //        orb.Content);
            //}

            ValidateCitizenBreath(
                citizen.Id,
                snapshot.Revision,
                breath);

            foreach (var cortexToolCall in breath.CortexToolCalls)
            {
                _logger.LogInformation(
                    "\x1b[35mCitizen\x1b[0m {CitizenId} produced Cortex tool call {CortexToolCall}.",
                    citizen.Id,
                    cortexToolCall);

                await _cortexToolCallExecutor.ExecuteAsync(
                    citizen.Id,
                    cortexToolCall,
                    cancellationToken);
            }

            foreach (var prediction in
                     breath.EmbodiedPredictions.Predictions)
            {
                _logger.LogInformation(
                    "\x1b[35mEntity\x1b[0m {EntityId} predicted {Prediction}.",
                    citizen.Id,
                    prediction);

                await _predictionExecutor.ExecuteAsync(
                    citizen,
                    nearbyEntityBindings,
                    prediction,
                    cancellationToken);
            }
        }

        private async Task EnsureAtMostOnePrimedCortexTaskAsync(
            string citizenId,
            DateTimeOffset now,
            CancellationToken cancellationToken)
        {
            var existingPrimedTask =
                await _primedCortexTaskContextLoader
                    .LoadAsync(
                        citizenId,
                        cancellationToken);

            if (existingPrimedTask is not null)
            {
                return;
            }

            if (!string.Equals(
                    citizenId,
                    "orestes",
                    StringComparison.Ordinal))
            {
                return;
            }

            var result =
                await _cortexTaskPrimingService
                    .TryPrimeNextEligibleCortexTaskAsync(
                        KnownCortexJobEmployment
                            .OrestesSimulateCitizenPassQualificationId,
                        now,
                        cancellationToken);

            _logger.LogInformation(
                "\x1b[35mCitizen\x1b[0m {CitizenId} Cortex priming produced " +
                "outcome {Outcome}, task {TaskId}, " +
                "assignment {AssignmentId}.",
                citizenId,
                result.Outcome,
                result.CortexTaskId,
                result.CortexTaskAssignmentId);
        }

        private static void ValidateCitizenBreath(
            string expectedCitizenId,
            long expectedObservedRevision,
            CitizenBreath breath)
        {
            ArgumentNullException.ThrowIfNull(
                breath);

            if (breath.EmbodiedPredictions is null)
            {
                throw new InvalidOperationException(
                    "\x1b[35mCitizen\x1b[0m breath contains no embodied prediction batch.");
            }

            ValidatePredictionBatch(
                expectedCitizenId,
                expectedObservedRevision,
                breath.EmbodiedPredictions);

            if (breath.CortexToolCalls is null)
            {
                throw new InvalidOperationException(
                    "\x1b[35mCitizen\x1b[0m breath contains a null Cortex tool-call list.");
            }

            if (breath.CortexToolCalls.Any(
                    call =>
                        call is null))
            {
                throw new InvalidOperationException(
                    "\x1b[35mCitizen\x1b[0m breath contains a null Cortex tool call.");
            }
        }

        private static void ValidatePredictionBatch(
            string expectedEntityId,
            long expectedObservedRevision,
            PredictionBatch batch)
        {
            ArgumentNullException.ThrowIfNull(
                batch);

            if (!string.Equals(
                    batch.EntityId,
                    expectedEntityId,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Controller for entity '{expectedEntityId}' " +
                    $"returned a batch for '{batch.EntityId}'.");
            }

            if (batch.BasedOnWorldRevision !=
                expectedObservedRevision)
            {
                throw new InvalidOperationException(
                    $"Controller for entity '{expectedEntityId}' " +
                    "returned an unexpected observed revision.");
            }

            if (batch.Predictions is null)
            {
                throw new InvalidOperationException(
                    $"Controller for entity '{expectedEntityId}' " +
                    "returned a null prediction list.");
            }

            if (batch.Predictions.Any(
                    prediction =>
                        prediction is null))
            {
                throw new InvalidOperationException(
                    $"Controller for entity '{expectedEntityId}' " +
                    "returned a null prediction.");
            }
        }
    }
}
