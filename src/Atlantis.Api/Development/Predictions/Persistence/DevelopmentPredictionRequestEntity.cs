namespace Atlantis.Api.Development.Predictions.Persistence;

public sealed class DevelopmentPredictionRequestEntity
{
    public Guid Id { get; set; }

    public required string DevelopmentWorldId
    {
        get;
        set;
    }

    public required string ScenarioId
    {
        get;
        set;
    }

    public required string SimulatedCitizenId
    {
        get;
        set;
    }

    public long SequenceNumber
    {
        get;
        set;
    }

    public required string ContextSerialized
    {
        get;
        set;
    }

    public required string ContextHash
    {
        get;
        set;
    }

    public int Status
    {
        get;
        set;
    }

    public int ContextContractVersion
    {
        get;
        set;
    }

    public DateTimeOffset CreatedAt
    {
        get;
        set;
    }
}