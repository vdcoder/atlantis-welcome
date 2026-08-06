using CStaticContext = Atlantis.Api.Citizens.Brain.CortexContext.StaticContext.StaticContext;
using CDynamicContext = Atlantis.Api.Citizens.Brain.CortexContext.DynamicContext.DynamicContext;
using CMemoryContext = Atlantis.Api.Citizens.Brain.CortexContext.MemoryContext.MemoryContext;
using CEchoContext = Atlantis.Api.Citizens.Brain.CortexContext.EchoContext.EchoContext;

namespace Atlantis.Api.Citizens.Brain.CortexContext;

public sealed class CortexContext
{
    public required CStaticContext StaticContext
    {
        get;
        init;
    }

    public required CDynamicContext DynamicContext
    {
        get;
        init;
    }

    public required CMemoryContext Memory
    {
        get;
        init;
    }

    public required CEchoContext Echo
    {
        get;
        init;
    }
}