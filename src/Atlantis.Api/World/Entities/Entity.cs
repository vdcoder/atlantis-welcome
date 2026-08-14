using Atlantis.Api.Common;
using Atlantis.Api.World.VisualAttributes;
using Atlantis.Api.World.VoiceAttributes;

namespace Atlantis.Api.World.Entities
{
    public class Entity
    {
        public string Id { get; set; } =
            string.Empty;

        public string Type { get; set; } =
            string.Empty;

        public string Name { get; set; } =
            string.Empty;

        public string PlaceId { get; set; } =
            string.Empty;

        public Position Position { get; set; } =
            new(0f, 0f, 0f);

        public DateTimeOffset PositionChangedAt { get; set; }

        public Embodiment? Embodiment { get; set; }

        public void FaceAndLook(
            Direction direction)
        {
            if (Embodiment is null)
            {
                throw new InvalidOperationException(
                    $"Entity '{Id}' is not embodied.");
            }

            var normalized =
                direction.Normalize();

            Embodiment.TorsoFront =
                normalized;

            Embodiment.GazeDirection =
                normalized;
        }

        public IReadOnlyList<VisualAttribute> VisualAttributes
        {
            get;
            set;
        } = [];

        public IReadOnlyList<VoiceAttribute> VoiceAttributes
        {
            get;
            set;
        } = [];
    }
}
