using System.Text.Json;

namespace Atlantis.Api.Development.Predictions.Contracts;

public sealed record RecordedCortexToolCallDto
{
    public required string Type
    {
        get;
        init;
    }

    public required JsonElement Arguments
    {
        get;
        init;
    }
}