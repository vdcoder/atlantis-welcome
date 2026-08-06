namespace Atlantis.Api.Models.Orbs;

public sealed record SensoryOrb : WorldOrb
{
    public SensoryOrb(
        Guid id,
        DateTimeOffset createdAt,
        DateTimeOffset? expiresAt,
        string? attachmentEntityId,
        Position attachmentPosition,
        float radius,
        SensoryModality modality,
        string content,
        float intensity)
        : base(
            id,
            createdAt,
            expiresAt,
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

        Modality = modality;
        Content = content;
        Intensity = intensity;
    }

    public SensoryModality Modality { get; }

    public string Content { get; }

    public float Intensity { get; }
}