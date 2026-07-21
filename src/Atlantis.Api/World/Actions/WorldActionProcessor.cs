using Atlantis.Api.Models;
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

                SayRequest sayRequest =>
                    ProcessAsync(sayRequest),

                TouchRequest touchRequest =>
                    ProcessAsync(touchRequest),

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

            var transition = new EntityMovedTransition(
                entity.Id,
                entity.Position,
                request.Destination);

            await _transitionProcessor.ApplyAsync(
                transition,
                _state);

            return [transition];
        }

        private async Task<IReadOnlyList<WorldTransition>> ProcessAsync(
    SayRequest request)
        {
            // Verify actor exists
            _ = _state.World.Entities
                .SingleOrDefault(entity =>
                    entity.Id == request.ActorId)
                ?? throw new EntityNotFoundException(
                    request.ActorId);

            // Verify entity exists
            var entity = _state.World.Entities
                .SingleOrDefault(entity =>
                    entity.Id == request.EntityId)
                ?? throw new EntityNotFoundException(
                    request.EntityId);

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                throw new ArgumentException(
                    "Utterance text cannot be empty.",
                    nameof(request.Text));
            }

            var utterance = new Utterance
            {
                Sequence = _state.Revision + 1,
                Text = request.Text,
                SpokenAt = DateTimeOffset.UtcNow
            };

            var transition = new EntitySpokeTransition(
                entity.Id,
                utterance);

            await _transitionProcessor.ApplyAsync(
                transition,
                _state);

            return [transition];
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
                    request.ActorId);

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

            if (target.Type is "citizen" or "visitor")
            {
                var message = new PrivateMessage
                {
                    Sequence = _state.Revision + 1,
                    SenderId = actor.Id,
                    Text = request.Text,
                    DeliveredAt = DateTimeOffset.UtcNow
                };

                var transition =
                    new PrivateMessageDeliveredTransition(
                        target.Id,
                        message);

                await _transitionProcessor.ApplyAsync(
                    transition,
                    _state);

                return [transition];
            }

            if (target.Type == "ui")
            {
                var transition =
                    new UiInputReceivedTransition(
                        target.Id,
                        actor.Id,
                        request.Text);

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

