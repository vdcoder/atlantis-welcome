using Atlantis.Api.Models;

namespace Atlantis.Api.World
{
    public sealed class MoveEntityResult
    {
        public long Revision { get; }
        public string EntityId { get; }
        public Position PreviousPosition { get; }
        public Position Position { get; }

        public MoveEntityResult(long revision, string entityId, Position previousPosition, Position position)
        {
            Revision = revision;
            EntityId = entityId;
            PreviousPosition = previousPosition;
            Position = position;
        }
    }
}
