using Atlantis.Api.Models;

namespace Atlantis.Api.World.Transitions
{
    public sealed record EntitySpokeTransition(
        string EntityId,
        Utterance Utterance)
        : WorldTransition;
}
