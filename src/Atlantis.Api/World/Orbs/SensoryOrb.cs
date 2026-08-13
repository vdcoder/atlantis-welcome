using Atlantis.Api.Common;

namespace Atlantis.Api.World.Orbs;

public sealed record SensoryOrb : Orb
{
    public SensoryOrb(
    Guid id,
    DateTimeOffset createdAt,
    DateTimeOffset? expiresAt,
    string? sourceEntityId,
    string? attachmentEntityId,
    Position attachmentPosition,
    float radius,
    SensoryModality modality,
    string content,
    float intensity,
    IReadOnlySet<string>? targetEntityIds = null)
    : base(
        id,
        createdAt,
        expiresAt,
        sourceEntityId,
        attachmentEntityId,
        attachmentPosition,
        radius)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            content);

        if (!float.IsFinite(intensity) ||
            intensity < 0f ||
            intensity > 1f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(intensity),
                "Sensory intensity must be between 0 and 1.");
        }

        if (targetEntityIds is not null)
        {
            if (targetEntityIds.Count == 0)
            {
                throw new ArgumentException(
                    "A targeted sensory orb must contain " +
                    "at least one target entity.",
                    nameof(targetEntityIds));
            }

            if (targetEntityIds.Any(
                string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException(
                    "Target entity IDs cannot be empty.",
                    nameof(targetEntityIds));
            }
        }

        Modality =
            modality;

        Content =
            content;

        Intensity =
            intensity;

        TargetEntityIds =
            targetEntityIds;
    }

    public SensoryModality Modality { get; }

    public string Content { get; }

    public float Intensity { get; }

    public IReadOnlySet<string>? TargetEntityIds { get; }
}