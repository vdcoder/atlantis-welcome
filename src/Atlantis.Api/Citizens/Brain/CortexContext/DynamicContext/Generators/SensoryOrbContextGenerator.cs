using Atlantis.Api.Citizens.Perception;
using Atlantis.Api.Common;

namespace Atlantis.Api.Citizens.Brain.CortexContext.DynamicContext.Generators;

public sealed class SensoryOrbContextGenerator
    : IDynamicContextGenerator
{
    private readonly IReadOnlyList<
        TransparentAuditoryEvent>
        _auditoryEvents;

    public SensoryOrbContextGenerator(
        IReadOnlyList<TransparentAuditoryEvent>
            auditoryEvents)
    {
        _auditoryEvents =
            auditoryEvents ??
            throw new ArgumentNullException(
                nameof(auditoryEvents));
    }

    public int PrefixStabilityHint =>
        750;

    public ValueTask GenerateAsync(
        ContextWriter writer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            writer);

        if (_auditoryEvents.Count == 0)
        {
            return ValueTask.CompletedTask;
        }

        writer.WriteLine(
            "<current_sensory_experience>");

        foreach (var auditoryEvent in
                 _auditoryEvents)
        {
            cancellationToken
                .ThrowIfCancellationRequested();

            writer.WriteLine(
                $"  <auditory_event " +
                $"ref=\"{Escape(auditoryEvent.Reference)}\" " +
                $"direction_x=\"{auditoryEvent.Direction.Direction.X:F2}\" " +
                $"direction_y=\"{auditoryEvent.Direction.Direction.Y:F2}\" " +
                $"direction_z=\"{auditoryEvent.Direction.Direction.Z:F2}\" " +
                $"volume=\"{auditoryEvent.Volume:F2}\">");

            writer.WriteLine(
                "    <content>");

            writer.WriteEscaped(
                auditoryEvent.Content);

            if (!auditoryEvent.Content.EndsWith(
                    Environment.NewLine,
                    StringComparison.Ordinal))
            {
                writer.WriteLine();
            }

            writer.WriteLine(
                "    </content>");

            writer.WriteLine(
                "  </auditory_event>");
        }

        writer.WriteLine(
            "</current_sensory_experience>");

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