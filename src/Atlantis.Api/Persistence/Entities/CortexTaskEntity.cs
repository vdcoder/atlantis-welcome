namespace Atlantis.Api.Persistence.Entities;

public sealed class CortexTaskEntity
{
    public Guid Id { get; set; }

    public Guid CortexJobDefinitionId { get; set; }

    public required string Instructions { get; set; }

    public decimal Reward { get; set; }

    public required string Currency { get; set; }

    public required string CompletionInboxId { get; set; }

    public required string FailureInboxId { get; set; }

    public int Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? AvailableAt { get; set; }

    public DateTimeOffset? UnavailableAt { get; set; }

    public CortexJobDefinitionEntity CortexJobDefinition
    {
        get;
        set;
    } = null!;

    public SimulateCitizenPassTaskEntity?
        SimulateCitizenPassTask
    {
        get;
        set;
    }

    public CortexTaskAssignmentEntity? Assignment
    {
        get;
        set;
    }
}