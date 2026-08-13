namespace Atlantis.Api.Common;

public static class TransformPosition
{
    public static Position TransformLocalPosition(
        Position worldPosition,
        Direction worldDirection,
        Position localPosition)
    {
        ArgumentNullException.ThrowIfNull(worldPosition);
        ArgumentNullException.ThrowIfNull(worldDirection);
        ArgumentNullException.ThrowIfNull(localPosition);

        var forward =
            worldDirection.Normalize();

        var up =
            Direction.UnitY;

        var rightUnnormalized =
            Direction.Cross(
                up,
                forward);

        if (rightUnnormalized.LengthSquared <
            Direction.MinimumLengthSquared)
        {
            throw new InvalidOperationException(
                "World direction is parallel to world-up.");
        }

        var right =
            rightUnnormalized.Normalize();

        return new Position(
            worldPosition.X +
                right.X * localPosition.X +
                up.X * localPosition.Y +
                forward.X * localPosition.Z,

            worldPosition.Y +
                right.Y * localPosition.X +
                up.Y * localPosition.Y +
                forward.Y * localPosition.Z,

            worldPosition.Z +
                right.Z * localPosition.X +
                up.Z * localPosition.Y +
                forward.Z * localPosition.Z);
    }
}