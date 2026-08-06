namespace Atlantis.Api.Development.Predictions.Remote;

public sealed record TrueWorldPredictionRequestDto
{
    public required Guid RequestId
    {
        get;
        init;
    }

    public required string DevelopmentWorldId
    {
        get;
        init;
    }

    public required string ScenarioId
    {
        get;
        init;
    }

    public required string SimulatedCitizenId
    {
        get;
        init;
    }

    public required long SequenceNumber
    {
        get;
        init;
    }

    public required int ContextContractVersion
    {
        get;
        init;
    }

    public required string ContextHash
    {
        get;
        init;
    }

    public required string ContextSerialized
    {
        get;
        init;
    }
}