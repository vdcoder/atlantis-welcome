namespace Atlantis.Api.Persistence.Entities;

public sealed class CortexJobScheduleConditionEntity
{
    public Guid Id { get; set; }

    public Guid CortexJobDefinitionId { get; set; }

    public required string ConditionType { get; set; }

    public required string ConfigurationJson { get; set; }

    public int SortOrder { get; set; }

    public CortexJobDefinitionEntity? CortexJobDefinition
    {
        get;
        set;
    }
}