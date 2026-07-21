using Atlantis.Api.Citizens.Perception;

namespace Atlantis.Api.Citizens.Interaction
{
    public sealed record TargetMatch(
        string EntityId,
        string Name,
        float Score,
        float Distance);

    public sealed class SemanticReach
    {
        public TargetMatch? Resolve(
            string query,
            IReadOnlyList<PerceivedObject> nearbyObjects)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return null;
            }

            var normalizedQuery = Normalize(query);

            return nearbyObjects
                .Where(item => item.IsInteractable)
                .Select(item => new TargetMatch(
                    item.EntityId,
                    item.Name,
                    Score(normalizedQuery, item),
                    item.Distance))
                .Where(match => match.Score > 0)
                .OrderByDescending(match => match.Score)
                .ThenBy(match => match.Distance)
                .FirstOrDefault();
        }

        private static string Normalize(string value)
        {
            return value
                .Trim()
                .ToLowerInvariant();
        }

        private static float Score(
            string normalizedQuery,
            PerceivedObject item)
        {
            var id = Normalize(item.EntityId);
            var name = Normalize(item.Name);
            var type = Normalize(item.Type);

            if (id == normalizedQuery)
                return 100f;

            if (name == normalizedQuery)
                return 90f;

            if (name.Contains(normalizedQuery))
                return 70f;

            if (normalizedQuery.Contains(name))
                return 60f;

            if (type == normalizedQuery)
                return 40f;

            var queryTokens = normalizedQuery
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var candidateTokens = $"{name} {type} {id}"
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToHashSet();

            var overlap = queryTokens.Count(candidateTokens.Contains);

            return overlap == 0
                ? 0f
                : 10f + overlap * 5f;
        }
    }
}
