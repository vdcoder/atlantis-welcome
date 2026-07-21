namespace Atlantis.Api.Models
{
    public class Entity
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string PlaceId { get; set; }
        public Position Position { get; set; }
        public Utterance? CurrentUtterance { get; set; }
        public PrivateMessage? CurrentPrivateMessage { get; set; }
    }
}
