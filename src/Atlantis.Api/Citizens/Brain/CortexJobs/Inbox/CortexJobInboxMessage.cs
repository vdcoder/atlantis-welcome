namespace Atlantis.Api.Citizens.Brain.CortexJobs.Inbox;

public sealed record CortexJobInboxMessage
{
    public required Guid Id { get; init; }

    public required string InboxId { get; init; }

    public required Guid CortexTaskId { get; init; }

    public required Guid CortexTaskAssignmentId { get; init; }

    public required CortexJobInboxMessageType Type { get; init; }

    public required string PayloadType { get; init; }

    public required string PayloadSerialized { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? ProcessedAt { get; init; }
}