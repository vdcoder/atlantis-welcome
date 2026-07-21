using Atlantis.Api.Models;
using Atlantis.Api.World;
using Atlantis.Api.World.Actions;
using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.Citizens.Brain.BrainTools
{
    public sealed class Touch
    {
        private readonly WorldActionProcessor _actionProcessor;

        public Touch(
            WorldActionProcessor actionProcessor)
        {
            _actionProcessor = actionProcessor;
        }

        public Task<IReadOnlyList<WorldTransition>> TouchAsync(
            string citizenId,
            string targetEntityId,
            string text)
        {
            var request = new TouchRequest(
                citizenId,
                targetEntityId,
                text);

            return _actionProcessor.ProcessAsync(request);
        }
    }
}
