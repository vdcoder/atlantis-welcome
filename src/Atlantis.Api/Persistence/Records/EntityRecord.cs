namespace Atlantis.Api.Persistence.Records
{
    public class EntityRecord
    {
        public string EntityId { get; set; } = string.Empty;
        public Guid WorldId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PlaceId { get; set; } = string.Empty;
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public DateTime PositionChangedAt { get; set; }

        public WorldRecord? World { get; set; }

        public float? TorsoFrontX { get; set; }
        public float? TorsoFrontY { get; set; }
        public float? TorsoFrontZ { get; set; }

        public float? GazeDirectionX { get; set; }
        public float? GazeDirectionY { get; set; }
        public float? GazeDirectionZ { get; set; }

        public ICollection<EntityVisualAttributeRecord>
            VisualAttributes { get; set; } = new List<EntityVisualAttributeRecord>();

        public ICollection<EntityVoiceAttributeRecord>
            VoiceAttributes { get; set; } = new List<EntityVoiceAttributeRecord>();

    }
}
