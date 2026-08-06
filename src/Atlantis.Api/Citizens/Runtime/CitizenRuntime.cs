using Atlantis.Api.Citizens.Brain;
using Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;
using Atlantis.Api.Citizens.Brain.CortexJobs.Context;
using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;
using Atlantis.Api.Citizens.Control;
using Atlantis.Api.Citizens.Control.EmbodiedTools;
using Atlantis.Api.Citizens.Interaction;
using Atlantis.Api.Citizens.Perception;
using Atlantis.Api.Models;
using Atlantis.Api.World;
using Atlantis.Api.World.EmbodiedControl;

namespace Atlantis.Api.Citizens.Runtime
{
    public sealed class CitizenRuntime
    {
        private readonly WorldRuntime _worldRuntime;
        private readonly MoveTo _moveTool;
        private readonly Say _sayTool;
        private readonly Touch _touchTool;
        private readonly TurnTorso _turnTorsoTool;
        private readonly SetGaze _setGazeTool;
        private readonly FaceAndLook _faceAndLookTool;
        private readonly NearbyEntityPerception _perception;
        private readonly SemanticReach _semanticReach;
        private readonly CitizenControllerRegistry _controllerRegistry;
        private readonly CortexToolCallExecutor _cortexToolCallExecutor;
        private readonly CortexTaskPrimingService _cortexTaskPrimingService;

        private readonly IPrimedCortexTaskContextLoader _primedCortexTaskContextLoader;
        private readonly SensoryOrbPerception _sensoryOrbPerception;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CitizenRuntime> _logger;

        public CitizenRuntime(
            WorldRuntime worldRuntime,
            MoveTo moveTool,
            Say sayTool,
            Touch touchTool,
            TurnTorso turnTorsoTool,
            SetGaze setGazeTool,
            FaceAndLook faceAndLookTool,
            NearbyEntityPerception perception,
            SemanticReach semanticReach,
            CitizenControllerRegistry controllerRegistry,
            CortexToolCallExecutor cortexToolCallExecutor,
            CortexTaskPrimingService cortexTaskPrimingService,
            IPrimedCortexTaskContextLoader primedCortexTaskContextLoader,
            SensoryOrbPerception sensoryOrbPerception,
            IServiceProvider serviceProvider,
            ILogger<CitizenRuntime> logger)
        {
            _worldRuntime = worldRuntime;
            _moveTool = moveTool;
            _sayTool = sayTool;
            _touchTool = touchTool;
            _turnTorsoTool = turnTorsoTool;
            _setGazeTool = setGazeTool;
            _faceAndLookTool = faceAndLookTool;
            _perception = perception;
            _semanticReach = semanticReach;
            _controllerRegistry = controllerRegistry;

            _cortexTaskPrimingService =
                cortexTaskPrimingService ??
                throw new ArgumentNullException(
                    nameof(cortexTaskPrimingService));

            _primedCortexTaskContextLoader =
                primedCortexTaskContextLoader ??
                throw new ArgumentNullException(
                    nameof(primedCortexTaskContextLoader));

            _cortexToolCallExecutor = cortexToolCallExecutor;
            _sensoryOrbPerception = sensoryOrbPerception;
            _serviceProvider = serviceProvider;
            _logger = logger;
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

            var nearbyObjects =
                _perception.Perceive(
                    citizen,
                    snapshot.World);

            var sensoryOrbs =
                _sensoryOrbPerception.Perceive(
                    citizen,
                    snapshot.World,
                    observedAt);

            var controllerContext =
                new EmbodiedControllerContext
                {
                    Entity =
                        citizen,

                    ObservedWorldRevision =
                        snapshot.Revision,

                    ObservedAt =
                        observedAt,

                    NearbyObjects =
                        nearbyObjects,

                    SensoryOrbs =
                        sensoryOrbs,
                };

            var breath =
                await controller.ProduceCitizenBreathAsync(
                    controllerContext,
                    cancellationToken);

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

                await ExecutePredictionAsync(
                    citizen,
                    nearbyObjects,
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

        private async Task ExecutePredictionAsync(
            Entity entity,
            IReadOnlyList<PerceivedObject> nearbyObjects,
            Prediction prediction,
            CancellationToken cancellationToken = default)
        {
            switch (prediction)
            {
                case WaitPrediction:
                    return;

                case MovePrediction move:
                    await _moveTool.MoveToAsync(
                        entity.Id,
                        move.X,
                        move.Z);
                    return;

                case SayPrediction say:
                    await _sayTool.SayAsync(
                        entity.Id,
                        say.Text);
                    return;

                case TouchPrediction touch:
                    await ProcessTouchPredictionAsync(
                        entity,
                        nearbyObjects,
                        touch);
                    return;

                case TurnTorsoPrediction turn:
                    await _turnTorsoTool.TurnTorsoAsync(
                        entity.Id,
                        turn.TorsoFront);
                    return;

                case SetGazePrediction gaze:
                    await _setGazeTool.SetGazeAsync(
                        entity.Id,
                        gaze.GazeDirection);
                    return;

                case FaceAndLookPrediction faceAndLook:
                    await _faceAndLookTool.FaceAndLookAsync(
                        entity.Id,
                        faceAndLook.Direction);
                    return;

                default:
                    throw new NotSupportedException(
                        $"Unsupported prediction type " +
                        $"'{prediction.GetType().Name}'.");
            }
        }

        private async Task ProcessTouchPredictionAsync(
            Entity entity,
            IReadOnlyList<PerceivedObject> nearbyObjects,
            TouchPrediction prediction)
        {
            var match =
                _semanticReach.Resolve(
                    prediction.TargetQuery,
                    nearbyObjects);

            if (match is null)
            {
                _logger.LogWarning(
                    "Entity {EntityId} could not resolve touch target " +
                    "'{TargetQuery}'.",
                    entity.Id,
                    prediction.TargetQuery);

                return;
            }

            _logger.LogInformation(
                "Semantic Reach resolved '{Query}' to {TargetId} " +
                "at {Distance:F2}m with score {Score}.",
                prediction.TargetQuery,
                match.EntityId,
                match.Distance,
                match.Score);

            await _touchTool.TouchAsync(
                entity.Id,
                match.EntityId,
                prediction.Text);
        }
    }
}
