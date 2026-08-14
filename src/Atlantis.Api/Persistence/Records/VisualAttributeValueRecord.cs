namespace Atlantis.Api.Persistence.Records;

public sealed class VisualAttributeValueRecord
{
    public int Id { get; set; }

    public int AttributeDefinitionId { get; set; }

    public VisualAttributeDefinitionRecord AttributeDefinition
    {
        get;
        set;
    } = null!;

    public string Value { get; set; } =
        string.Empty;

    public string DisplayText { get; set; } =
        string.Empty;
}