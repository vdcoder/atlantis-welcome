namespace Atlantis.Api.Citizens.Brain.CortexContext.DynamicContext;

public sealed class DynamicContext
    : ContextArea<IDynamicContextGenerator>
{
    public DynamicContext(
        IEnumerable<IDynamicContextGenerator> generators)
        : base(generators)
    {
    }
}