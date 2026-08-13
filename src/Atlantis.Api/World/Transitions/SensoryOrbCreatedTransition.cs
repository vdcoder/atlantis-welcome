using Atlantis.Api.World.Orbs;

namespace Atlantis.Api.World.Transitions;

public sealed record SensoryOrbCreatedTransition(
    SensoryOrb Orb)
    : WorldTransition;