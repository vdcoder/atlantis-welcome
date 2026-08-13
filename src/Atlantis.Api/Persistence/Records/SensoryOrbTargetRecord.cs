namespace Atlantis.Api.Persistence.Records;

public sealed class SensoryOrbTargetRecord
{
    public Guid OrbId { get; set; }

    public string TargetEntityId { get; set; } =
        string.Empty;

    public SensoryOrbRecord? SensoryOrb { get; set; }
}