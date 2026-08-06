using Atlantis.Api.Models;
using Atlantis.Api.Models.Orbs;
using Atlantis.Api.World.Orbs;

namespace Atlantis.Api.Citizens.Perception;

public sealed class SensoryOrbPerception
{
    private readonly WorldOrbCollection
        _orbCollection;

    private readonly WorldOrbPositionResolver
        _positionResolver;

    public SensoryOrbPerception(
        WorldOrbCollection orbCollection,
        WorldOrbPositionResolver positionResolver)
    {
        _orbCollection =
            orbCollection ??
            throw new ArgumentNullException(
                nameof(orbCollection));

        _positionResolver =
            positionResolver ??
            throw new ArgumentNullException(
                nameof(positionResolver));
    }

    public IReadOnlyList<PerceivedSensoryOrb> Perceive(
        Entity observer,
        Models.World world,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(
            observer);

        ArgumentNullException.ThrowIfNull(
            world);

        return _orbCollection
            .GetActiveAt(now)
            .OfType<SensoryOrb>()
            .Select(orb =>
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
                    Distance = distance
                };
            })
            .Where(item =>
                item.Distance <=
                item.Orb.Radius)
            .OrderBy(item =>
                item.Distance)
            .ThenBy(item =>
                item.Orb.CreatedAt)
            .Select(item =>
                new PerceivedSensoryOrb(
                    item.Orb.Id,
                    item.Orb.Modality,
                    item.Orb.Content,
                    item.Orb.Intensity,
                    item.Distance))
            .ToArray();
    }

    private static float Distance(
        Position left,
        Position right)
    {
        var dx =
            left.X - right.X;

        var dy =
            left.Y - right.Y;

        var dz =
            left.Z - right.Z;

        return MathF.Sqrt(
            dx * dx +
            dy * dy +
            dz * dz);
    }
}