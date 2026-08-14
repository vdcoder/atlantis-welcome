namespace Atlantis.Api.Citizens.Brain.Memory.WorkingMemory
{
    public sealed record EasyLogEntry(
        Guid Id,
        string Content,
        int ContentTokenCount,
        DateTimeOffset CreatedAt)
    {
        public int BudgetTokenCount =>
            ContentTokenCount +
            WorkingMemoryLimits.EasyLogEntryHeaderTokens;
    }
}