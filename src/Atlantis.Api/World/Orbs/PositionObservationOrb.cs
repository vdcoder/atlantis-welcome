using Atlantis.Api.Common;

namespace Atlantis.Api.World.Orbs;

public sealed record PositionObservationOrb : Orb
{
    public PositionObservationOrb(
        Guid id,
        DateTimeOffset createdAt,
        DateTimeOffset? expiresAt,
        string sourceEntityId,
        string observedEntityId,
        Position observedPosition,
        DateTimeOffset positionFirstRecordedAt)
        : base(
            id,
            createdAt,
            expiresAt,
            sourceEntityId,
            attachmentEntityId: null,
            attachmentPosition: observedPosition,
            radius: 0f)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            observedEntityId);

        if (positionFirstRecordedAt > createdAt)
        {
            throw new ArgumentOutOfRangeException(
                nameof(positionFirstRecordedAt),
                "The position cannot have first been recorded " +
                "after the observation orb was created.");
        }

        ObservedEntityId =
            observedEntityId;

        PositionFirstRecordedAt =
            positionFirstRecordedAt;
    }

    public string ObservedEntityId { get; }

    public Position ObservedPosition =>
        AttachmentPosition;

    public DateTimeOffset PositionFirstRecordedAt { get; }
}