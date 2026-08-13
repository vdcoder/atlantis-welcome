namespace Atlantis.Api.Persistence.Records;

public sealed class SimulateCitizenPassWorkOrderRecord
{
    public Guid Id
    {
        get;
        set;
    }

    public Guid CortexTaskId
    {
        get;
        set;
    }

    public Guid ExternalRequestId
    {
        get;
        set;
    }

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

    public int ContextContractVersion
    {
        get;
        set;
    }

    public required string ContextHash
    {
        get;
        set;
    }

    public required string ContextSerialized
    {
        get;
        set;
    }

    public int Status
    {
        get;
        set;
    }

    public DateTimeOffset CreatedAt
    {
        get;
        set;
    }

    public DateTimeOffset? CompletedAt
    {
        get;
        set;
    }

    public SimulateCitizenPassTaskRecord
        SimulateCitizenPassTask
    {
        get;
        set;
    } = null!;
}