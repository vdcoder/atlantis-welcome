namespace Atlantis.Api.Models;

public sealed class Embodiment
{
    public Direction TorsoFront { get; set; } =
        Direction.UnitZ;

    public Direction GazeDirection { get; set; } =
        Direction.UnitZ;
}