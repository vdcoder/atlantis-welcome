namespace Atlantis.Api.Persistence.Records;

public sealed class PositionObservationOrbRecord
{
    public Guid OrbId { get; set; }

    public string ObservedEntityId { get; set; } =
        string.Empty;

    public DateTime PositionFirstRecordedAt { get; set; }

    public OrbRecord? Orb { get; set; }
}