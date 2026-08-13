namespace Atlantis.Api.Persistence.Records;

public sealed class DevelopmentRecordedPredictionRecord
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

    public required string ContextHash
    {
        get;
        set;
    }

    public required string CognitiveOutputSerialized
    {
        get;
        set;
    }

    public required string WorkerCitizenId
    {
        get;
        set;
    }

    public int ContextContractVersion
    {
        get;
        set;
    }

    public DateTimeOffset RecordedAt
    {
        get;
        set;
    }
}