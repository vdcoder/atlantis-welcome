namespace Atlantis.Api.Citizens.Brain.CortexContext;

public interface IContextGenerator
{
    /// <summary>
    /// Lower values are emitted earlier and form the more stable
    /// prefix of the context area.
    /// </summary>
    int PrefixStabilityHint => 500;

    ValueTask GenerateAsync(
        ContextWriter writer,
        CancellationToken cancellationToken = default);
}