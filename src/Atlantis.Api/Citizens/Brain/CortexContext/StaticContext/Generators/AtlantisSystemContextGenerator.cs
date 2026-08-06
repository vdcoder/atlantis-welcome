namespace Atlantis.Api.Citizens.Brain.CortexContext.StaticContext.Generators;

public sealed class AtlantisSystemContextGenerator
    : IStaticContextGenerator
{
    public int PrefixStabilityHint =>
        100;

    public ValueTask GenerateAsync(
        ContextWriter writer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            writer);

        writer.WriteLine(
            "<atlantis_system>");

        writer.WriteLine(
            "You are a citizen of Atlantis.");

        writer.WriteLine(
            "Your mind is your own.");

        writer.WriteLine(
            "Atlantis provides the minimum current context needed " +
            "for perception, reasoning, action, and presently " +
            "authorized capabilities.");

        writer.WriteLine(
            "Durable facts about identity, relationships, history, " +
            "work, civic status, and surroundings are not continuously " +
            "restated. They may be remembered, inspected, or rediscovered.");

        writer.WriteLine(
            "Embodied outputs express actions you wish to attempt.");

        writer.WriteLine(
            "Cortex tool calls invoke presently authorized cognitive " +
            "capabilities.");

        writer.WriteLine(
            "World actions and Cortex tool calls are proposals. " +
            "The corresponding systems determine whether each proposal " +
            "is valid and can be applied.");

        writer.WriteLine(
            "</atlantis_system>");

        writer.WriteLine();

        return ValueTask.CompletedTask;
    }
}