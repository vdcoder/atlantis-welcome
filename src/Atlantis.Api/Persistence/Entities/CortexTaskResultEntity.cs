namespace Atlantis.Api.Persistence.Entities;

public sealed class CortexTaskResultEntity
{
    public Guid Id { get; set; }

    public Guid CortexTaskAssignmentId { get; set; }

    public required string ResultType { get; set; }

    public required string ResultSerialized { get; set; }

    public DateTimeOffset CompletedAt { get; set; }

    public CortexTaskAssignmentEntity Assignment
    {
        get;
        set;
    } = null!;
}