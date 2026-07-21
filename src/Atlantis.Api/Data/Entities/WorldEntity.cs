namespace Atlantis.Api.Data.Entities
{
    public class WorldEntity
    {
        public Guid Id { get; set; }
        public string WorldId { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public long Revision { get; set; }

        public ICollection<EntityEntity> Entities { get; set; } = [];
    }
}
