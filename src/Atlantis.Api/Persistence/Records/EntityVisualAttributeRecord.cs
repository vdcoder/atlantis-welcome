namespace Atlantis.Api.Persistence.Records;

public sealed class EntityVisualAttributeRecord
{
    public string EntityId { get; set; } =
        string.Empty;

    public EntityRecord Entity { get; set; } =
        null!;

    public int AttributeValueId { get; set; }

    public VisualAttributeValueRecord AttributeValue
    {
        get;
        set;
    } = null!;

    public int Sequence { get; set; }
}