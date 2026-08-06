namespace Atlantis.Api.Development.Predictions.Requests;

public sealed record DevelopmentPredictionRequestContext
{
    public required string BodyEntityId
    {
        get;
        init;
    }

    public required long ObservedWorldRevision
    {
        get;
        init;
    }

    public required DateTimeOffset ObservedAt
    {
        get;
        init;
    }
}