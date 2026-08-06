using Atlantis.Api.Citizens.Perception;

namespace Atlantis.Api.Citizens.Brain.CortexContext.DynamicContext.Generators;

public sealed class SensoryOrbContextGenerator
    : IDynamicContextGenerator
{
    private readonly IReadOnlyList<
        PerceivedSensoryOrb>
        _sensoryOrbs;

    public SensoryOrbContextGenerator(
        IReadOnlyList<PerceivedSensoryOrb>
            sensoryOrbs)
    {
        _sensoryOrbs =
            sensoryOrbs ??
            throw new ArgumentNullException(
                nameof(sensoryOrbs));
    }

    public int PrefixStabilityHint =>
        750;

    public ValueTask GenerateAsync(
        ContextWriter writer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            writer);

        if (_sensoryOrbs.Count == 0)
        {
            return ValueTask.CompletedTask;
        }

        writer.WriteLine(
            "<current_sensory_experience>");

        foreach (var orb in _sensoryOrbs)
        {
            cancellationToken
                .ThrowIfCancellationRequested();

            writer.WriteLine(
                "  <sensory_event>");

            writer.WriteLine(
                $"    <modality>{orb.Modality}</modality>");

            writer.WriteLine(
                $"    <intensity>{orb.Intensity:F2}</intensity>");

            writer.WriteLine(
                $"    <distance_meters>{orb.Distance:F2}</distance_meters>");

            writer.WriteLine(
                "    <content>");

            writer.WriteRaw(
                orb.Content);

            if (!orb.Content.EndsWith(
                    Environment.NewLine,
                    StringComparison.Ordinal))
            {
                writer.WriteLine();
            }

            writer.WriteLine(
                "    </content>");

            writer.WriteLine(
                "  </sensory_event>");
        }

        writer.WriteLine(
            "</current_sensory_experience>");

        writer.WriteLine();

        return ValueTask.CompletedTask;
    }
}