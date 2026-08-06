namespace Atlantis.Api.Persistence.Entities;

public sealed class CortexJobDefinitionEntity
{
    public Guid Id { get; set; }

    public required string EmployerAccountId { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string Qualifications { get; set; }

    public required string CompletionInboxId { get; set; }

    public required string FailureInboxId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeactivatedAt { get; set; }

    public bool IsActiveAt(DateTimeOffset time) =>
        DeactivatedAt is null ||
        time < DeactivatedAt;

    public ICollection<CortexJobScheduleConditionEntity>
        ScheduleConditions
    {
        get;
        set;
    } = [];
}