namespace Atlantis.Api.Persistence.Records;

public sealed class EntityVoiceAttributeRecord
{
    public string EntityId { get; set; } =
        string.Empty;

    public EntityRecord Entity { get; set; } =
        null!;

    public int AttributeValueId { get; set; }

    public VoiceAttributeValueRecord AttributeValue
    {
        get;
        set;
    } = null!;

    public int Sequence { get; set; }
}