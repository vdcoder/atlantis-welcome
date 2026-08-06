using Atlantis.Api.Models.Orbs;

namespace Atlantis.Api.Citizens.Perception
{
    public sealed record PerceivedSensoryOrb(
        Guid OrbId,
        SensoryModality Modality,
        string Content,
        float Intensity,
        float Distance);
}
