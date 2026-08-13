using Atlantis.Api.Common;
using Atlantis.Api.World.Entities;
using Atlantis.Api.World.Orbs;

namespace Atlantis.Api.Citizens.Perception;

public sealed record TransparentPosition(
    Position Position,
    DateTimeOffset FirstRecordedAt);

public sealed record TransparentEntity(
    string Reference,
    string Type,
    float Distance,
    bool IsInteractable,
    Position Position,
    IReadOnlyList<TransparentPosition> RecentPositions);

public sealed record PerceivedEntityBinding(
    string EntityId,
    TransparentEntity TransparentEntity);

public sealed class NearbyEntityPerception
{
    private const float PerceptionRadius = 8f;

    public IReadOnlyList<PerceivedEntityBinding> Perceive(
        Entity observer,
        World.World world,
        DateTimeOffset observedAt)
    {
        ArgumentNullException.ThrowIfNull(observer);
        ArgumentNullException.ThrowIfNull(world);

        return world.Entities
            .Where(
                entity =>
                    entity.Id != observer.Id)
            .Select(
                entity =>
                {
                    var distance =
                        Distance(
                            observer.Position,
                            entity.Position);

                    return new
                    {
                        Entity = entity,
                        Distance = distance
                    };
                })
            .Where(
                item =>
                    item.Distance <=
                    PerceptionRadius)
            .OrderBy(
                item =>
                    item.Distance)
            .Select(
                item =>
                {
                    var transparentEntity =
                        new TransparentEntity(
                            Reference:
                                BuildReference(
                                    item.Entity),

                            Type:
                                item.Entity.Type,

                            Distance:
                                item.Distance,

                            IsInteractable:
                                IsInteractable(
                                    item.Entity),

                            Position:
                                item.Entity.Position,

                            RecentPositions:
                                GetRecentPositions(
                                    item.Entity,
                                    world,
                                    observedAt));

                    return new PerceivedEntityBinding(
                        EntityId:
                            item.Entity.Id,

                        TransparentEntity:
                            transparentEntity);
                })
            .ToList();
    }

    private static string BuildReference(
        Entity entity)
    {
        return string.Join(
            " ",
            entity.VisualAttributes
                .OrderBy(
                    attribute =>
                        attribute.Sequence)
                .Select(
                    attribute =>
                        attribute.DisplayText));
    }

    private static IReadOnlyList<TransparentPosition>
        GetRecentPositions(
            Entity entity,
            World.World world,
            DateTimeOffset observedAt)
    {
        return world.Orbs
            .OfType<PositionObservationOrb>()
            .Where(
                orb =>
                    orb.ObservedEntityId ==
                        entity.Id &&
                    orb.IsActiveAt(
                        observedAt))
            .OrderBy(
                orb =>
                    orb.PositionFirstRecordedAt)
            .ThenBy(
                orb =>
                    orb.CreatedAt)
            .ThenBy(
                orb =>
                    orb.Id)
            .Select(
                orb =>
                    new TransparentPosition(
                        orb.ObservedPosition,
                        orb.PositionFirstRecordedAt))
            .ToList();
    }

    private static bool IsInteractable(
        Entity entity)
    {
        return entity.Type is
            "citizen" or
            "visitor" or
            "ui";
    }

    private static float Distance(
        Position left,
        Position right)
    {
        var dx = left.X - right.X;
        var dy = left.Y - right.Y;
        var dz = left.Z - right.Z;

        return MathF.Sqrt(
            dx * dx +
            dy * dy +
            dz * dz);
    }
}
