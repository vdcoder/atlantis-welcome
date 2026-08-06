using Atlantis.Api.Models;

namespace Atlantis.Api.Citizens.Brain.CortexContext.DynamicContext.Generators;

public sealed class CurrentPositionContextGenerator
    : IDynamicContextGenerator
{
    private readonly Position _position;

    public CurrentPositionContextGenerator(
        Position position)
    {
        _position = position;
    }

    public int PrefixStabilityHint => 700;

    public ValueTask GenerateAsync(
        ContextWriter writer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(writer);

        writer.WriteLine(
            "<current_embodied_state>");

        writer.WriteLine(
            $"  <position " +
            $"x=\"{_position.X:F1}\" " +
            $"y=\"{_position.Y:F1}\" " +
            $"z=\"{_position.Z:F1}\" />");

        writer.WriteLine(
            "</current_embodied_state>");

        writer.WriteLine();

        return ValueTask.CompletedTask;
    }
}