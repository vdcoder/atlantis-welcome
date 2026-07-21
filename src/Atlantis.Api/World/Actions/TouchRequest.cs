namespace Atlantis.Api.World.Actions
{
    public sealed record TouchRequest(
        string ActorId,
        string TargetEntityId,
        string Text)
        : WorldActionRequest(ActorId);
}
