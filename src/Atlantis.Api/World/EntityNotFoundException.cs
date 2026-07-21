namespace Atlantis.Api.World
{
    public sealed class EntityNotFoundException : Exception
    {
        public string EntityId { get; }

        public EntityNotFoundException(string entityId)
            : base($"Entity with id '{entityId}' was not found.")
        {
            EntityId = entityId;
        }
    }
}
