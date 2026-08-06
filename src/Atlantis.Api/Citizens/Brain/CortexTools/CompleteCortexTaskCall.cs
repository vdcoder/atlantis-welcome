namespace Atlantis.Api.Citizens.Brain.CortexTools;

public sealed record CompleteCortexTaskCall
    : CortexToolCall
{
    public required Guid AssignmentId
    {
        get;
        init;
    }

    public required string ResultSerialized
    {
        get;
        init;
    }
}