using Atlantis.Api.Common;
using Atlantis.Api.World.Entities;
using Atlantis.Api.World.Orbs;
using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.World.Actions
{
    public sealed class WorldActionProcessor
    {
        private readonly WorldState _state;
        private readonly WorldTransitionProcessor _transitionProcessor;
        private readonly ILogger<WorldActionProcessor> _logger;

        public WorldActionProcessor(
            WorldState state,
            WorldTransitionProcessor transitionProcessor,
            ILogger<WorldActionProcessor> logger)
        {
            _state = state;
            _transitionProcessor = transitionProcessor;
            _logger = logger;
        }

        public Task<IReadOnlyList<WorldTransition>> ProcessAsync(WorldActionRequest request)
        {
            return request switch
            {
                MoveEntityRequest moveRequest =>
                    ProcessAsync(moveRequest),

                TurnEntityTorsoRequest turnTorsoRequest =>
                    ProcessAsync(turnTorsoRequest),

                SetEntityGazeRequest setGazeRequest =>
                    ProcessAsync(setGazeRequest),

                FaceAndLookRequest faceAndLookRequest =>
                    ProcessAsync(faceAndLookRequest),

                SayRequest sayRequest =>
                    ProcessAsync(sayRequest),

                TouchRequest touchRequest =>
                    ProcessAsync(touchRequest),

                ReportPositionObservationRequest positionObservationRequest =>
                    ProcessAsync(positionObservationRequest),

                _ => throw new NotSupportedException(
                    $"Unsupported world action request type: " +
                    $"{request.GetType().Name}.")
            };
        }

        private async Task<IReadOnlyList<WorldTransition>> ProcessAsync(MoveEntityRequest request)
        {
            var entity = _state.World.Entities
                .SingleOrDefault(entity => entity.Id == request.EntityId)
                ?? throw new EntityNotFoundException(request.EntityId);

            var transition =
                new EntityMovedTransition(
                    entity.Id,
                    entity.Position,
                    request.Destination,
                    DateTimeOffset.UtcNow);

            await _transitionProcessor.ApplyAsync(
                transition,
                _state);

            return [transition];
        }

        private async Task<IReadOnlyList<WorldTransition>> ProcessAsync(
            SayRequest request)
        {
            if (request.ActorId is not null)
            {
                _ = _state.World.Entities
                    .SingleOrDefault(
                        entity =>
                            entity.Id == request.ActorId)
                    ?? throw new EntityNotFoundException(
                        request.ActorId);
            }

            _ = _state.World.Entities
                .SingleOrDefault(
                    entity =>
                        entity.Id == request.EntityId)
                ?? throw new EntityNotFoundException(
                    request.EntityId);

            if (string.IsNullOrWhiteSpace(
                    request.Text))
            {
                throw new ArgumentException(
                    "Say text cannot be empty.",
                    nameof(request.Text));
            }

            var now =
                request.SpokenAt ??
                DateTimeOffset.UtcNow;

            var sensoryOrb =
                new SensoryOrb(
                    id:
                        Guid.NewGuid(),

                    createdAt:
                        now,

                    expiresAt:
                        now.AddSeconds(60),

                    sourceEntityId:
                        request.ActorId,

                    attachmentEntityId:
                        request.EntityId,

                    attachmentPosition:
                        new Position(
                            0f,
                            1.6f,
                            0.15f),

                    radius:
                        10f,

                    modality:
                        SensoryModality.Auditory,

                    content:
                        request.Text,

                    intensity:
                        0.5f);

            var orbTransition =
                new SensoryOrbCreatedTransition(
                    sensoryOrb);

            var transitions =
                new WorldTransition[]
                {
                    orbTransition
                };

            await _transitionProcessor.ApplyAsync(
                transitions,
                _state);

            return transitions;
        }

        private const float MaximumTouchDistance = 2f;
        private const int MaximumTouchTextLength = 2_000;

        private async Task<IReadOnlyList<WorldTransition>> ProcessAsync(
            TouchRequest request)
        {
            var actor = _state.World.Entities
                .SingleOrDefault(entity =>
                    entity.Id == request.ActorId)
                ?? throw new EntityNotFoundException(
                    request.ActorId ?? "");

            var target = _state.World.Entities
                .SingleOrDefault(entity =>
                    entity.Id == request.TargetEntityId)
                ?? throw new EntityNotFoundException(
                    request.TargetEntityId);

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                throw new ArgumentException(
                    "Touch text cannot be empty.",
                    nameof(request.Text));
            }

            if (request.Text.Length > MaximumTouchTextLength)
            {
                throw new ArgumentException(
                    $"Touch text cannot exceed " +
                    $"{MaximumTouchTextLength} characters.",
                    nameof(request.Text));
            }

            var distance = Distance(
                actor.Position,
                target.Position);

            if (distance > MaximumTouchDistance)
            {
                _logger.LogInformation(
                    "Entity {ActorId} attempted to touch {TargetId} " +
                    "at {Distance:F2}m; maximum touch distance is " +
                    "{MaximumDistance:F2}m. No transition was produced.",
                    actor.Id,
                    target.Id,
                    distance,
                    MaximumTouchDistance);

                return [];
            }

            if (target.Type is
                "citizen" or
                "visitor" or
                "ui")
            {
                var now =
                    DateTimeOffset.UtcNow;

                var orb =
                    new SensoryOrb(
                        id:
                            Guid.NewGuid(),

                        createdAt:
                            now,

                        expiresAt:
                            now.AddSeconds(1),

                        sourceEntityId:
                            actor.Id,

                        attachmentEntityId:
                            null,

                        attachmentPosition:
                            target.Position,

                        radius:
                            MaximumTouchDistance,

                        modality:
                            SensoryModality.Tactile,

                        content:
                            request.Text,

                        intensity:
                            0.5f,

                        targetEntityIds:
                            new HashSet<string>
                            {
                                target.Id
                            });

                var transition =
                    new SensoryOrbCreatedTransition(
                        orb);

                await _transitionProcessor.ApplyAsync(
                    transition,
                    _state);

                return [transition];
            }

            _logger.LogInformation(
                "Entity {ActorId} attempted to touch {TargetId}, " +
                "but entity type {TargetType} does not accept touch input. " +
                "No transition was produced.",
                actor.Id,
                target.Id,
                target.Type);

            return [];
        }

        private async Task<IReadOnlyList<WorldTransition>> ProcessAsync(
            TurnEntityTorsoRequest request)
        {
            var entity =
                GetAuthorizedEmbodiedTarget(
                    request.ActorId ?? "",
                    request.TargetEntityId);

            var torsoFront =
                NormalizeHorizontalDirection(
                    request.TorsoFront);

            var transition =
                new EntityTorsoTurnedTransition(
                    entity.Id,
                    torsoFront);

            await _transitionProcessor.ApplyAsync(
                transition,
                _state);

            return [transition];
        }

        private async Task<IReadOnlyList<WorldTransition>> ProcessAsync(
            SetEntityGazeRequest request)
        {
            var entity =
                GetAuthorizedEmbodiedTarget(
                    request.ActorId ?? "",
                    request.TargetEntityId);

            var gazeDirection =
                request.GazeDirection.Normalize();

            var transition =
                new EntityGazeChangedTransition(
                    entity.Id,
                    gazeDirection);

            await _transitionProcessor.ApplyAsync(
                transition,
                _state);

            return [transition];
        }

        private async Task<IReadOnlyList<WorldTransition>> ProcessAsync(
            FaceAndLookRequest request)
        {
            var entity =
                GetAuthorizedEmbodiedTarget(
                    request.ActorId ?? "",
                    request.TargetEntityId);

            var torsoFront =
                NormalizeHorizontalDirection(
                    request.Direction);

            var gazeDirection =
                request.Direction.Normalize();

            var transitions =
                new WorldTransition[]
                {
                    new EntityTorsoTurnedTransition(
                        entity.Id,
                        torsoFront),

                    new EntityGazeChangedTransition(
                        entity.Id,
                        gazeDirection)
                };

            await _transitionProcessor.ApplyAsync(
                transitions,
                _state);

            return transitions;
        }

        private const float
            PositionChangeToleranceMeters =
                0.001f;

        private static bool HasPositionChanged(
            Position current,
            Position reported)
        {
            var dx =
                current.X - reported.X;

            var dy =
                current.Y - reported.Y;

            var dz =
                current.Z - reported.Z;

            var distanceSquared =
                dx * dx +
                dy * dy +
                dz * dz;

            return distanceSquared >
                PositionChangeToleranceMeters *
                PositionChangeToleranceMeters;
        }

        private async Task<IReadOnlyList<WorldTransition>> ProcessAsync(
            ReportPositionObservationRequest request)
        {
            var actor =
                _state.World.Entities
                    .SingleOrDefault(
                        entity =>
                            entity.Id == request.ActorId)
                ?? throw new EntityNotFoundException(
                    request.ActorId ?? "");

            var observedEntity =
                _state.World.Entities
                    .SingleOrDefault(
                        entity =>
                            entity.Id == request.ObservedEntityId)
                ?? throw new EntityNotFoundException(
                    request.ObservedEntityId);

            if (actor.Id != observedEntity.Id)
            {
                throw new InvalidOperationException(
                    "An entity may currently report only " +
                    "its own position.");
            }

            if (!HasPositionChanged(
                observedEntity.Position,
                request.Position))
            {
                return [];
            }

            var transition =
                new EntityPositionReportedTransition(
                    EntityId:
                        observedEntity.Id,

                    SourceEntityId:
                        actor.Id,

                    ReportedPosition:
                        request.Position,

                    ObservedAt:
                        DateTimeOffset.UtcNow);

            await _transitionProcessor.ApplyAsync(
                transition,
                _state);

            return [transition];
        }

        private Entity GetAuthorizedEmbodiedTarget(
            string actorId,
            string targetEntityId)
        {
            _ =
                _state.World.Entities
                    .SingleOrDefault(
                        entity =>
                            entity.Id == actorId)
                ?? throw new EntityNotFoundException(
                    actorId);

            var target =
                _state.World.Entities
                    .SingleOrDefault(
                        entity =>
                            entity.Id == targetEntityId)
                ?? throw new EntityNotFoundException(
                    targetEntityId);

            if (actorId != targetEntityId)
            {
                throw new InvalidOperationException(
                    "An entity may currently orient only itself.");
            }

            if (target.Embodiment is null)
            {
                throw new InvalidOperationException(
                    $"Entity '{target.Id}' is not embodied.");
            }

            return target;
        }

        private static Direction NormalizeHorizontalDirection(
            Direction direction)
        {
            ArgumentNullException.ThrowIfNull(direction);

            return new Direction(
                direction.X,
                0f,
                direction.Z)
                .Normalize();
        }

        private static float Distance(
            Position left,
            Position right)
        {
            var dx = left.X - right.X;
            var dy = left.Y - right.Y;
            var dz = left.Z - right.Z;

            return MathF.Sqrt(
                dx * dx +
                dy * dy +
                dz * dz);
        }
    }
}

