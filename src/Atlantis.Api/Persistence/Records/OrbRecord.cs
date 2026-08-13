namespace Atlantis.Api.Persistence.Records;

public sealed class OrbRecord
{
    public Guid Id { get; set; }

    public Guid WorldId { get; set; }

    public string Type { get; set; } =
        string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public string? SourceEntityId { get; set; }

    public string? AttachmentEntityId { get; set; }

    public float AttachmentPositionX { get; set; }

    public float AttachmentPositionY { get; set; }

    public float AttachmentPositionZ { get; set; }

    public float Radius { get; set; }

    public WorldRecord? World { get; set; }

    public SensoryOrbRecord? SensoryOrb { get; set; }

    public PositionObservationOrbRecord? PositionObservationOrb { get; set; }
}