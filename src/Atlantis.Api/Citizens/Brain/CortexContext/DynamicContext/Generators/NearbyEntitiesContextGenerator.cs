namespace Atlantis.Api.Citizens.Brain.CortexContext.DynamicContext.Generators;

using Atlantis.Api.Citizens.Perception;

public sealed class NearbyEntitiesContextGenerator
    : IDynamicContextGenerator
{
    private readonly IReadOnlyList<TransparentEntity>
        _nearbyEntities;

    public NearbyEntitiesContextGenerator(
        IReadOnlyList<TransparentEntity> nearbyEntities)
    {
        _nearbyEntities =
            nearbyEntities ??
            throw new ArgumentNullException(
                nameof(nearbyEntities));
    }

    public int PrefixStabilityHint =>
        725;

    public IReadOnlyList<TransparentEntity>
        NearbyEntities =>
            _nearbyEntities;

    public ValueTask GenerateAsync(
        ContextWriter writer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            writer);

        if (_nearbyEntities.Count == 0)
        {
            return ValueTask.CompletedTask;
        }

        writer.WriteLine(
            "<nearby_entities>");

        foreach (var transparentEntity in
         _nearbyEntities)
        {
            cancellationToken
                .ThrowIfCancellationRequested();

            writer.WriteLine(
                $"  <entity " +
                $"ref=\"{Escape(transparentEntity.Reference)}\" " +
                $"type=\"{Escape(transparentEntity.Type)}\" " +
                $"distance_meters=\"{transparentEntity.Distance:F2}\" " +
                $"interactable=\"{transparentEntity.IsInteractable.ToString().ToLowerInvariant()}\">");

            writer.WriteLine(
                $"    <position " +
                $"x=\"{transparentEntity.Position.X:F2}\" " +
                $"y=\"{transparentEntity.Position.Y:F2}\" " +
                $"z=\"{transparentEntity.Position.Z:F2}\" />");

            if (transparentEntity.RecentPositions.Count > 0)
            {
                writer.WriteLine(
                    "    <recent_positions>");

                foreach (var recentPosition in
                         transparentEntity.RecentPositions)
                {
                    writer.WriteLine(
                        $"      <position " +
                        $"x=\"{recentPosition.Position.X:F2}\" " +
                        $"y=\"{recentPosition.Position.Y:F2}\" " +
                        $"z=\"{recentPosition.Position.Z:F2}\" " +
                        $"since=\"{recentPosition.FirstRecordedAt:O}\" />");
                }

                writer.WriteLine(
                    "    </recent_positions>");
            }

            writer.WriteLine(
                "  </entity>");
        }

        writer.WriteLine(
            "</nearby_entities>");

        writer.WriteLine();

        return ValueTask.CompletedTask;
    }

    private static string Escape(
        string value)
    {
        return System.Security.SecurityElement
            .Escape(value)
            ?? string.Empty;
    }
}