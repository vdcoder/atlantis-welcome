namespace Atlantis.Api.World
{
    public sealed class EmbodimentNotFoundException : Exception
    {
        public string EntityId { get; }

        public EmbodimentNotFoundException(string entityId)
            : base($"Embodiment with id '{entityId}' was not found.")
        {
            EntityId = entityId;
        }
    }
}
