namespace Atlantis.Api.Persistence.Records;

public sealed class CortexTaskResultRecord
{
    public Guid Id { get; set; }

    public Guid CortexTaskAssignmentId { get; set; }

    public required string ResultType { get; set; }

    public required string ResultSerialized { get; set; }

    public DateTimeOffset CompletedAt { get; set; }

    public CortexTaskAssignmentRecord Assignment
    {
        get;
        set;
    } = null!;
}