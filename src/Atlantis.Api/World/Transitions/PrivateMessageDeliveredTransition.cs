using Atlantis.Api.Models;

namespace Atlantis.Api.World.Transitions
{
    public sealed record PrivateMessageDeliveredTransition(
        string RecipientId,
        PrivateMessage Message)
        : WorldTransition;
}
