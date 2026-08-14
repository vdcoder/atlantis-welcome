using Atlantis.Api.Common;

namespace Atlantis.Api.World.Orbs;

public abstract record Orb
{
    protected Orb(
        Guid id,
        DateTimeOffset createdAt,
        DateTimeOffset? expiresAt,
        string? sourceEntityId,
        string? attachmentEntityId,
        Position attachmentPosition,
        float radius)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "A world orb must have a non-empty ID.",
                nameof(id));
        }

        if (expiresAt is not null &&
            expiresAt <= createdAt)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expiresAt),
                "A world orb cannot expire before " +
                "or when it is created.");
        }

        if (!float.IsFinite(radius) ||
            radius < 0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(radius),
                "A world orb radius must be finite " +
                "and non-negative.");
        }

        Id =
            id;

        CreatedAt =
            createdAt;

        ExpiresAt =
            expiresAt;

        SourceEntityId =
            sourceEntityId;

        AttachmentEntityId =
            attachmentEntityId;

        AttachmentPosition =
            attachmentPosition;

        Radius =
            radius;
    }

    public Guid Id { get; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? ExpiresAt { get; }

    public string? SourceEntityId { get; }

    public string? AttachmentEntityId { get; }

    public Position AttachmentPosition { get; }

    public float Radius { get; }

    public bool IsActiveAt(
        DateTimeOffset time)
    {
        return
            time >= CreatedAt &&
            !IsExpiredAt(time);
    }

    public bool IsExpiredAt(
        DateTimeOffset time)
    {
        return
            ExpiresAt is not null &&
            time >= ExpiresAt.Value;
    }
}