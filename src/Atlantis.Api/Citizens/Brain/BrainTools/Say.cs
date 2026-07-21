using Atlantis.Api.World;
using Atlantis.Api.World.Actions;
using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.Citizens.Brain.BrainTools
{
    public sealed class Say
    {
        private readonly WorldActionProcessor _actionProcessor;

        public Say(WorldActionProcessor actionProcessor)
        {
            _actionProcessor = actionProcessor;
        }

        public async Task<IReadOnlyList<WorldTransition>> SayAsync(
            string citizenId,
            string text)
        {
            var request = new SayRequest(
                citizenId,
                citizenId,
                text);

            return await _actionProcessor.ProcessAsync(request);
        }
    }
}
