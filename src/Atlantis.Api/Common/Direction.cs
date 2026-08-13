namespace Atlantis.Api.Common;

public sealed record Direction(
    float X,
    float Y,
    float Z)
{
    public const float MinimumLengthSquared =
        0.000001f;

    public float LengthSquared =>
        X * X +
        Y * Y +
        Z * Z;

    public Direction Normalize()
    {
        if (LengthSquared < MinimumLengthSquared)
        {
            throw new InvalidOperationException(
                "Direction cannot be normalized because it is zero.");
        }

        var inverseLength =
            1f / MathF.Sqrt(LengthSquared);

        return new Direction(
            X * inverseLength,
            Y * inverseLength,
            Z * inverseLength);
    }

    public static Direction UnitY =>
        new(0f, 1f, 0f);

    public static Direction UnitZ =>
        new(0f, 0f, 1f);

    public static Direction Cross(
        Direction left,
        Direction right)
    {
        return new Direction(
            left.Y * right.Z -
                left.Z * right.Y,

            left.Z * right.X -
                left.X * right.Z,

            left.X * right.Y -
                left.Y * right.X);
    }
}