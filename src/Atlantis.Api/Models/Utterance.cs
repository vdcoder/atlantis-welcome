namespace Atlantis.Api.Models
{
    public sealed class Utterance
    {
        public long Sequence { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTimeOffset SpokenAt { get; set; }
    }
}
