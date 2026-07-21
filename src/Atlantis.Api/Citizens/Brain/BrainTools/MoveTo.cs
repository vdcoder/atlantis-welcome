using Atlantis.Api.Models;
using Atlantis.Api.World;
using Atlantis.Api.World.Actions;
using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.Citizens.Brain.BrainTools
{
    public sealed class MoveTo
    {
        private readonly WorldActionProcessor _actionProcessor;

        public MoveTo(WorldActionProcessor actionProcessor)
        {
            _actionProcessor = actionProcessor;
        }

        public async Task<IReadOnlyList<WorldTransition>> MoveToAsync(
            string citizenId,
            float x,
            float z)
        {
            var request = new MoveEntityRequest(
                citizenId,
                citizenId,
                new Position(x, 0f, z));

            return await _actionProcessor.ProcessAsync(request);
        }
    }
}
