namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation.Contracts;

public sealed record SimulationPredictionDto
{
    public required string Type { get; init; }

    public float? X { get; init; }

    public float? Z { get; init; }

    public string? Text { get; init; }

    public string? TargetQuery { get; init; }

    public SimulationDirectionDto? Direction
    {
        get;
        init;
    }
}