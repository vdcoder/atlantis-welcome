namespace Atlantis.Api.Development.Predictions.Domain;

public sealed record DevelopmentPredictionReplayKey
{
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
}