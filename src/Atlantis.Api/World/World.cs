using Atlantis.Api.World.Entities;
using Atlantis.Api.World.Orbs;

namespace Atlantis.Api.World
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

        public List<Orb> Orbs { get; set; } = [];
    }
}
