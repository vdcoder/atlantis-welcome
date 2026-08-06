namespace Atlantis.Api.Models
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

        public Embodiment? Embodiment { get; set; }

        public Utterance? CurrentUtterance { get; set; }

        public PrivateMessage? CurrentPrivateMessage { get; set; }

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
    }
}
