namespace Atlantis.Api.Models
{
    public class World
    {
        public string WorldId { get; set; } =
            string.Empty;

        public DateTime Time { get; set; }

        public List<Place> Places { get; set; } =
            [];

        public List<Entity> Entities { get; set; } =
            [];
    }
}
