namespace Atlantis.Api.Persistence.Records
{
    public class WorldRecord
    {
        public Guid Id { get; set; }
        public string WorldId { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public long Revision { get; set; }

        public ICollection<EntityRecord> Entities { get; set; } = [];

        public ICollection<OrbRecord> Orbs { get; set; } = [];
    }
}
