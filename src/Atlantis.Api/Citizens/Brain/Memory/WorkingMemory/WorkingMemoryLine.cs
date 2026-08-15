namespace Atlantis.Api.Citizens.Brain.Memory.WorkingMemory
{
    public sealed record WorkingMemoryLine(
        Guid Id,
        int LineNumber,
        string Content,
        int ContentTokenCount,
        DateTimeOffset CreatedAt);
}