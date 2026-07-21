namespace Atlantis.Api.Models
{
    public sealed class PrivateMessage
    {
        public long Sequence { get; set; }

        public string SenderId { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public DateTimeOffset DeliveredAt { get; set; }
    }
}
