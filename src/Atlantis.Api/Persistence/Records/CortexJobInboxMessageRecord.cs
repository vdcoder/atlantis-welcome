namespace Atlantis.Api.Persistence.Records;

public sealed class CortexJobInboxMessageRecord
{
    public Guid Id { get; set; }

    public required string InboxId { get; set; }

    public Guid CortexTaskId { get; set; }

    public Guid CortexTaskAssignmentId { get; set; }

    public int Type { get; set; }

    public required string PayloadType { get; set; }

    public required string PayloadSerialized { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ProcessedAt { get; set; }

    public CortexTaskRecord CortexTask
    {
        get;
        set;
    } = null!;

    public CortexTaskAssignmentRecord Assignment
    {
        get;
        set;
    } = null!;
}