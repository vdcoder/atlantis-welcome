namespace Atlantis.Api.World.Orbs;

public sealed record TextSensoryOrbPayload
{
    public string Text { get; init; } =
        string.Empty;
}