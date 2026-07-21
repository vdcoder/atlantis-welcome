using Atlantis.Api.Citizens.Perception;

namespace Atlantis.Api.Citizens.AgentKernel
{
    public sealed class Context
    {
        public string StaticContext { get; init; } = string.Empty;
        public string DynamicContext { get; init; } = string.Empty;
        public string Memory { get; init; } = string.Empty;
        public string RecentHistory { get; init; } = string.Empty;

        // TEMP

        public required string CitizenId { get; init; }

        public required IReadOnlyList<PerceivedObject> NearbyObjects
        {
            get;
            init;
        }
    }
}
