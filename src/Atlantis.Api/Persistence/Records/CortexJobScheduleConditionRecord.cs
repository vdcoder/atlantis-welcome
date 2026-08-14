namespace Atlantis.Api.Persistence.Records;

public sealed class CortexJobScheduleConditionRecord
{
    public Guid Id { get; set; }

    public Guid CortexJobDefinitionId { get; set; }

    public required string ConditionType { get; set; }

    public required string ConfigurationJson { get; set; }

    public int SortOrder { get; set; }

    public CortexJobDefinitionRecord? CortexJobDefinition
    {
        get;
        set;
    }
}