using System.Text.Json;

namespace Atlantis.Api.Persistence.Records;

public sealed class SensoryOrbRecord
{
    public Guid OrbId { get; set; }

    public int Modality { get; set; }

    public float Intensity { get; set; }

    public JsonDocument Payload { get; set; } =
        JsonDocument.Parse("{}");

    public List<SensoryOrbTargetRecord> Targets { get; set; } = new List<SensoryOrbTargetRecord>();

    public OrbRecord? Orb { get; set; }
}