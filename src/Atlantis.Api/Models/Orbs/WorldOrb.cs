using Atlantis.Api.Models;

namespace Atlantis.Api.Models.Orbs;

public abstract record WorldOrb
{
    protected WorldOrb(
        Guid id,
        DateTimeOffset createdAt,
        DateTimeOffset? expiresAt,
        string? attachmentEntityId,
        Position attachmentPosition,
        float radius)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "World-orb ID cannot be empty.",
                nameof(id));
        }

        ArgumentNullException.ThrowIfNull(
            attachmentPosition);

        if (expiresAt is not null &&
            expiresAt <= createdAt)
        {
            throw new ArgumentException(
                "World-orb expiration must occur after creation.",
                nameof(expiresAt));
        }

        if (!float.IsFinite(radius) ||
            radius < 0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(radius),
                "World-orb radius must be finite and nonnegative.");
        }

        if (attachmentEntityId is not null &&
            string.IsNullOrWhiteSpace(
                attachmentEntityId))
        {
            throw new ArgumentException(
                "Attachment entity ID cannot be blank.",
                nameof(attachmentEntityId));
        }

        Id = id;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        AttachmentEntityId = attachmentEntityId;
        AttachmentPosition = attachmentPosition;
        Radius = radius;
    }

    public Guid Id { get; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? ExpiresAt { get; }

    /// <summary>
    /// Null means AttachmentPosition is expressed in world space.
    /// Otherwise, AttachmentPosition is expressed in the attached
    /// entity's torso-local coordinate frame.
    /// </summary>
    public string? AttachmentEntityId { get; }

    public Position AttachmentPosition { get; }

    public float Radius { get; }

    public bool IsExpiredAt(
        DateTimeOffset time)
    {
        return ExpiresAt is not null &&
               time >= ExpiresAt;
    }
}