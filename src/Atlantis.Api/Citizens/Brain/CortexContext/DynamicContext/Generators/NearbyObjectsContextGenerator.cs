namespace Atlantis.Api.Citizens.Brain.CortexContext.DynamicContext.Generators;

using Atlantis.Api.Citizens.Perception;

public sealed class NearbyObjectsContextGenerator
    : IDynamicContextGenerator
{
    private readonly IReadOnlyList<PerceivedObject>
        _nearbyObjects;

    public NearbyObjectsContextGenerator(
        IReadOnlyList<PerceivedObject> nearbyObjects)
    {
        _nearbyObjects =
            nearbyObjects ??
            throw new ArgumentNullException(
                nameof(nearbyObjects));
    }

    public int PrefixStabilityHint =>
        725;

    public IReadOnlyList<PerceivedObject>
        NearbyObjects =>
            _nearbyObjects;

    public ValueTask GenerateAsync(
        ContextWriter writer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            writer);

        if (_nearbyObjects.Count == 0)
        {
            return ValueTask.CompletedTask;
        }

        writer.WriteLine(
            "<nearby_objects>");

        foreach (var perceivedObject in
                 _nearbyObjects)
        {
            cancellationToken
                .ThrowIfCancellationRequested();

            writer.WriteLine(
                $"  <object " +
                $"name=\"{Escape(perceivedObject.Name)}\" " +
                $"type=\"{Escape(perceivedObject.Type)}\" " +
                $"distance_meters=\"{perceivedObject.Distance:F2}\" " +
                $"interactable=\"{perceivedObject.IsInteractable.ToString().ToLowerInvariant()}\" />");
        }

        writer.WriteLine(
            "</nearby_objects>");

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