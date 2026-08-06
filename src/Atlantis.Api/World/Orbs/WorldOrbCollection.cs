using Atlantis.Api.Models.Orbs;

namespace Atlantis.Api.World.Orbs;

public sealed class WorldOrbCollection
{
    private readonly object _gate =
        new();

    private readonly Dictionary<Guid, WorldOrb>
        _orbs =
            [];

    public void Add(
        WorldOrb orb)
    {
        ArgumentNullException.ThrowIfNull(orb);

        lock (_gate)
        {
            if (!_orbs.TryAdd(
                    orb.Id,
                    orb))
            {
                throw new InvalidOperationException(
                    $"World orb '{orb.Id}' already exists.");
            }
        }
    }

    public IReadOnlyList<WorldOrb> GetActiveAt(
        DateTimeOffset time)
    {
        lock (_gate)
        {
            RemoveExpiredUnsafe(time);

            return _orbs.Values
                .OrderBy(orb =>
                    orb.CreatedAt)
                .ThenBy(orb =>
                    orb.Id)
                .ToArray();
        }
    }

    public bool Remove(
        Guid orbId)
    {
        lock (_gate)
        {
            return _orbs.Remove(
                orbId);
        }
    }

    private void RemoveExpiredUnsafe(
        DateTimeOffset time)
    {
        var expiredIds =
            _orbs.Values
                .Where(orb =>
                    orb.IsExpiredAt(time))
                .Select(orb =>
                    orb.Id)
                .ToArray();

        foreach (var expiredId in expiredIds)
        {
            _orbs.Remove(
                expiredId);
        }
    }
}