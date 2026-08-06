namespace Atlantis.Api.Data.Entities
{
    public class EntityEntity
    {
        public Guid Id { get; set; }
        public Guid WorldId { get; set; }
        public string EntityId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PlaceId { get; set; } = string.Empty;
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public long? UtteranceSequence { get; set; }
        public string? UtteranceText { get; set; }
        public DateTime? UtteranceSpokenAt { get; set; }

        public WorldEntity? World { get; set; }
        public long? PrivateMessageSequence { get; set; }
        public string? PrivateMessageSenderId { get; set; }
        public string? PrivateMessageText { get; set; }
        public DateTime? PrivateMessageDeliveredAt { get; set; }

        public float? TorsoFrontX { get; set; }
        public float? TorsoFrontY { get; set; }
        public float? TorsoFrontZ { get; set; }

        public float? GazeDirectionX { get; set; }
        public float? GazeDirectionY { get; set; }
        public float? GazeDirectionZ { get; set; }

    }
}
