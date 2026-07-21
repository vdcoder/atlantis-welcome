using System;

namespace AtlantisWelcome.World
{
    [Serializable]
    public sealed class WorldSnapshotDto
    {
        public long revision;
        public WorldDto world;
    }

    [Serializable]
    public sealed class WorldDto
    {
        public string worldId;
        public string time;
        public PlaceDto[] places;
        public EntityDto[] entities;
    }

    [Serializable]
    public sealed class PlaceDto
    {
        public string id;
        public string name;
    }

    [Serializable]
    public sealed class EntityDto
    {
        public string id;
        public string type;
        public string name;
        public string placeId;
        public PositionDto position;

        public UtteranceDto currentUtterance;
        public PrivateMessageDto currentPrivateMessage;
    }

    [Serializable]
    public sealed class PositionDto
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    public sealed class UtteranceDto
    {
        public long sequence;
        public string text;
        public string spokenAt;
    }

    [Serializable]
    public sealed class PrivateMessageDto
    {
        public long sequence;
        public string senderId;
        public string text;
        public string deliveredAt;
    }

    [Serializable]
    public sealed class MoveEntityRequestDto
    {
        public string actorId = string.Empty;
        public string entityId = string.Empty;
        public PositionDto destination = new();
    }

    [Serializable]
    public sealed class MoveEntityResultDto
    {
        public long revision;
        public string entityId = string.Empty;
        public PositionDto previousPosition = new();
        public PositionDto position = new();
    }
}