namespace Atlantis.Api.Models.Spatial;

public static class EntityTransform
{
    public static Position TransformLocalPosition(
        Entity entity,
        Position localPosition)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(localPosition);

        if (entity.Embodiment is null)
        {
            throw new InvalidOperationException(
                $"Entity '{entity.Id}' has no embodiment orientation.");
        }

        var forward =
            entity.Embodiment.TorsoFront.Normalize();

        var up =
            Direction.UnitY;

        var rightUnnormalized =
            Cross(
                up,
                forward);

        if (rightUnnormalized.LengthSquared <
            Direction.MinimumLengthSquared)
        {
            throw new InvalidOperationException(
                $"Entity '{entity.Id}' has a torso-front direction " +
                "parallel to world-up.");
        }

        var right =
            rightUnnormalized.Normalize();

        return new Position(
            entity.Position.X +
                right.X * localPosition.X +
                up.X * localPosition.Y +
                forward.X * localPosition.Z,

            entity.Position.Y +
                right.Y * localPosition.X +
                up.Y * localPosition.Y +
                forward.Y * localPosition.Z,

            entity.Position.Z +
                right.Z * localPosition.X +
                up.Z * localPosition.Y +
                forward.Z * localPosition.Z);
    }

    private static Direction Cross(
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