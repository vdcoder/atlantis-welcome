namespace Atlantis.Api.Persistence.Records;

public sealed class VoiceAttributeValueRecord
{
    public int Id { get; set; }

    public int AttributeDefinitionId { get; set; }

    public VoiceAttributeDefinitionRecord AttributeDefinition
    {
        get;
        set;
    } = null!;

    public string Value { get; set; } =
        string.Empty;

    public string DisplayText { get; set; } =
        string.Empty;
}