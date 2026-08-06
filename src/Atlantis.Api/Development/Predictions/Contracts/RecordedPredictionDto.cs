namespace Atlantis.Api.Development.Predictions.Contracts;

public sealed record RecordedPredictionDto
{
    public required string Type
    {
        get;
        init;
    }

    public float? X
    {
        get;
        init;
    }

    public float? Z
    {
        get;
        init;
    }

    public string? Text
    {
        get;
        init;
    }

    public string? TargetQuery
    {
        get;
        init;
    }

    public RecordedDirectionDto? Direction
    {
        get;
        init;
    }
}