using Atlantis.Api.Models;
using Atlantis.Api.World.Actions;
using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.World
{
    public sealed class WorldRuntime
    {
        private readonly WorldState _worldState;
        private readonly WorldActionProcessor _actionProcessor;

        public WorldRuntime(
            WorldState worldState,
            WorldActionProcessor actionProcessor)
        {
            _worldState = worldState;
            _actionProcessor = actionProcessor;
        }

        public WorldSnapshot GetSnapshot()
        {
            return new WorldSnapshot(
                _worldState.Revision,
                _worldState.World);
        }

        public Entity? FindEntity(string entityId)
        {
            return _worldState.World.Entities
                .FirstOrDefault(entity => entity.Id == entityId);
        }

        public async Task<MoveEntityResult> MoveEntityAsync(
            string actorId,
            string entityId,
            Position destination)
        {
            var request = new MoveEntityRequest(
                actorId,
                entityId,
                destination);

            var transition = (EntityMovedTransition)await _actionProcessor.ProcessAsync(request);

            return new MoveEntityResult(
                _worldState.Revision,
                transition.EntityId,
                transition.From,
                transition.To);
        }
    }
}
