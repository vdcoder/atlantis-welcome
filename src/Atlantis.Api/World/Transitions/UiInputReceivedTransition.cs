namespace Atlantis.Api.World.Transitions
{
    public sealed record UiInputReceivedTransition(
        string TargetEntityId,
        string ActorId,
        string Input)
        : WorldTransition;
}
