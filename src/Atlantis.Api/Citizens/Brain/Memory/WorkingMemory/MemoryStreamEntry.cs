namespace Atlantis.Api.Citizens.Brain.Memory.WorkingMemory
{
    public sealed record MemoryStreamEntry(
        Guid Id,
        string Content,
        int ContentTokenCount,
        DateTimeOffset CreatedAt)
    {
        public int BudgetTokenCount =>
            ContentTokenCount +
            WorkingMemoryLimits
                .StreamEntryHeaderTokens;
    }
}