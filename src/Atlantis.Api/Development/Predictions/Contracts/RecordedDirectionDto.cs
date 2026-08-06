namespace Atlantis.Api.Development.Predictions.Contracts;

public sealed record RecordedDirectionDto
{
    public required float X
    {
        get;
        init;
    }

    public required float Y
    {
        get;
        init;
    }

    public required float Z
    {
        get;
        init;
    }
}