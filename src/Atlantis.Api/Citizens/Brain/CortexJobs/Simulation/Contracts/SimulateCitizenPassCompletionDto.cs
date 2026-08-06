namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation.Contracts;

public sealed record SimulateCitizenPassCompletionDto
{
    public required IReadOnlyList<SimulationPredictionDto> Predictions
    {
        get;
        init;
    }
}