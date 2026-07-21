using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.World
{
    public sealed class WorldTransitionProcessor
    {
        private readonly WorldPersistenceService _persistenceService;
        private readonly SemaphoreSlim _gate = new(1, 1);

        public WorldTransitionProcessor(WorldPersistenceService persistenceService)
        {
            _persistenceService = persistenceService;
        }

        public async Task ApplyAsync(
            WorldTransition transition,
            WorldState state,
            CancellationToken cancellationToken = default)
        {
            await _gate.WaitAsync(cancellationToken);

            try
            {
                ApplyTransition(transition, state);
                state.AdvanceRevision();

                await _persistenceService.SaveAsync(
                    state,
                    cancellationToken);
            }
            finally
            {
                _gate.Release();
            }
        }

        private void ApplyTransition(
            WorldTransition transition,
            WorldState state)
        {
            switch (transition)
            {
                case EntityMovedTransition movement:
                        Apply(movement, state);
                        break;

                case EntitySpokeTransition speech:
                    Apply(speech, state);
                    break;

                case PrivateMessageDeliveredTransition message:
                    Apply(message, state);
                    break;

                case UiInputReceivedTransition uiInput:
                    Apply(uiInput, state);
                    break;

                default:
                    throw new NotSupportedException(
                        $"Unsupported transition: {transition.GetType().Name}");
            }
        }

        private void Apply(
            EntityMovedTransition transition,
            WorldState state)
        {
            var entity = state.World.Entities
                .Single(entity => entity.Id == transition.EntityId);

            entity.Position = transition.To;
        }

        private void Apply(
            EntitySpokeTransition transition,
            WorldState state)
        {
            var entity = state.World.Entities
                .Single(entity => entity.Id == transition.EntityId);

            entity.CurrentUtterance = transition.Utterance;
        }

        private static void Apply(
            PrivateMessageDeliveredTransition transition,
            WorldState state)
        {
            var recipient = state.World.Entities
                .Single(entity =>
                    entity.Id == transition.RecipientId);

            recipient.CurrentPrivateMessage =
                transition.Message;
        }

        private static void Apply(
            UiInputReceivedTransition transition,
            WorldState state)
        {
            // log
        }
    }
}
