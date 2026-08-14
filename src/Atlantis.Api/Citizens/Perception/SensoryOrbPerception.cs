using Atlantis.Api.Common;
using Atlantis.Api.World.Entities;
using Atlantis.Api.World.Orbs;
using Atlantis.Api.World.Spatial;

namespace Atlantis.Api.Citizens.Perception;

public sealed record TransparentDirection(
    Direction Direction);

public sealed record TransparentAuditoryEvent(
    string Reference,
    TransparentDirection Direction,
    float Volume,
    string Content);

public sealed record PerceivedAuditoryEventBinding(
    Guid OrbId,
    TransparentAuditoryEvent TransparentAuditoryEvent);

public sealed class SensoryOrbPerception
{
    private static readonly Position
        HearingOffset =
            new(
                0f,
                1.6f,
                0f);

    private readonly OrbPositionResolver
        _positionResolver;

    private readonly ISensoryPathResolver
        _pathResolver;

    public SensoryOrbPerception(
        OrbPositionResolver positionResolver,
        ISensoryPathResolver pathResolver)
    {
        _positionResolver =
            positionResolver ??
            throw new ArgumentNullException(
                nameof(positionResolver));

        _pathResolver =
            pathResolver ??
            throw new ArgumentNullException(
                nameof(pathResolver));
    }

    public IReadOnlyList<PerceivedAuditoryEventBinding> Perceive(
        Entity observer,
        World.World world,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(observer);
        ArgumentNullException.ThrowIfNull(world);

        return world.Orbs
            .Where(
                orb =>
                    orb.IsActiveAt(now))
            .OfType<SensoryOrb>()
            .Where(
                orb =>
                    orb.Modality ==
                        SensoryModality.Auditory)
            .Where(
                orb =>
                    orb.TargetEntityIds is null ||
                    orb.TargetEntityIds.Contains(
                        observer.Id))
            .Select(
                orb =>
                {
                    var worldPosition =
                        _positionResolver.Resolve(
                            orb,
                            world);

                    var distance =
                        Distance(
                            observer.Position,
                            worldPosition);

                    return new
                    {
                        Orb = orb,
                        WorldPosition = worldPosition,
                        Distance = distance
                    };
                })
            .Where(
                item =>
                    item.Distance <=
                        item.Orb.Radius)
            .Where(
                item =>
                    _pathResolver.HasClearPath(
                        item.WorldPosition,
                        observer.Position,
                        world))
            .OrderBy(
                item =>
                    item.Distance)
            .ThenBy(
                item =>
                    item.Orb.CreatedAt)
            .ThenBy(
                item =>
                    item.Orb.Id)
            .Select(
                item =>
                {
                    var source =
                        item.Orb.SourceEntityId is null
                            ? null
                            : world.Entities.FirstOrDefault(
                                entity =>
                                    entity.Id ==
                                    item.Orb.SourceEntityId);

                    var reference =
                        source is null
                            ? string.Empty
                            : BuildVoiceReference(
                                source);

                    var hearingPosition =
                        new Position(
                            observer.Position.X +
                                HearingOffset.X,

                            observer.Position.Y +
                                HearingOffset.Y,

                            observer.Position.Z +
                                HearingOffset.Z);

                    var direction =
                        DirectionTo(
                            hearingPosition,
                            item.WorldPosition);

                    var volume =
                        PerceivedVolume(
                            item.Orb.Intensity,
                            item.Distance,
                            item.Orb.Radius);

                    return new PerceivedAuditoryEventBinding(
                        OrbId:
                            item.Orb.Id,

                        TransparentAuditoryEvent:
                            new TransparentAuditoryEvent(
                                Reference:
                                    reference,

                                Direction:
                                    new TransparentDirection(
                                        direction),

                                Volume:
                                    volume,

                                Content:
                                    item.Orb.Content));
                })
            .ToArray();
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

    private static string BuildVoiceReference(
    Entity entity)
    {
        return string.Join(
            " ",
            entity.VoiceAttributes
                .OrderBy(
                    attribute =>
                        attribute.Sequence)
                .Select(
                    attribute =>
                        attribute.DisplayText));
    }

    private static Direction DirectionTo(
        Position observer,
        Position source)
    {
        var direction =
            new Direction(
                source.X - observer.X,
                source.Y - observer.Y,
                source.Z - observer.Z);

        if (direction.LengthSquared <=
            0.000001f)
        {
            return Direction.UnitZ;
        }

        return direction.Normalize();
    }

    private static float PerceivedVolume(
        float intensity,
        float distance,
        float radius)
    {
        if (radius <= 0f)
        {
            return distance <= 0f
                ? intensity
                : 0f;
        }

        var attenuation =
            1f -
            Math.Clamp(
                distance / radius,
                0f,
                1f);

        return intensity *
            attenuation;
    }
}