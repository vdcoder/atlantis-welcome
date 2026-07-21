using Atlantis.Api.Models;

namespace Atlantis.Api.Citizens.Perception
{
    public sealed record PerceivedObject(
        string EntityId,
        string Name,
        string Type,
        float Distance,
        bool IsInteractable);

    public sealed class NearbyEntityPerception
    {
        private const float PerceptionRadius = 8f;

        public IReadOnlyList<PerceivedObject> Perceive(
            Entity observer,
            Models.World world)
        {
            return world.Entities
                .Where(entity => entity.Id != observer.Id)
                .Select(entity =>
                {
                    var distance = Distance(
                        observer.Position,
                        entity.Position);

                    return new
                    {
                        Entity = entity,
                        Distance = distance
                    };
                })
                .Where(item => item.Distance <= PerceptionRadius)
                .OrderBy(item => item.Distance)
                .Select(item => new PerceivedObject(
                    item.Entity.Id,
                    item.Entity.Name,
                    item.Entity.Type,
                    item.Distance,
                    IsInteractable(item.Entity)))
                .ToList();
        }

        private static bool IsInteractable(Entity entity)
        {
            return entity.Type is "citizen" or "visitor" or "ui";
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
}
