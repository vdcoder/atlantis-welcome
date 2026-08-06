namespace Atlantis.Api.Citizens.Brain.CortexContext.StaticContext;

public sealed class StaticContext
    : ContextArea<IStaticContextGenerator>
{
    public StaticContext(
        IEnumerable<IStaticContextGenerator> generators)
        : base(generators)
    {
    }
}