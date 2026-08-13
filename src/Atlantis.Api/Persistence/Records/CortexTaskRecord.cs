namespace Atlantis.Api.Persistence.Records;

public sealed class CortexTaskRecord
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

    public CortexJobDefinitionRecord CortexJobDefinition
    {
        get;
        set;
    } = null!;

    public SimulateCitizenPassTaskRecord?
        SimulateCitizenPassTask
    {
        get;
        set;
    }

    public CortexTaskAssignmentRecord? Assignment
    {
        get;
        set;
    }
}