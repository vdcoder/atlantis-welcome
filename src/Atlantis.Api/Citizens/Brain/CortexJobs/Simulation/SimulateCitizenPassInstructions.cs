namespace Atlantis.Api.Citizens.Brain.CortexJobs.Simulation;

public sealed record SimulateCitizenPassInstructions
{
    public required string CortexTaskType { get; init; }

    public required string ExternalScenarioId { get; init; }

    public required string
        PassContextAndCurrentPositionSerialized
    {
        get;
        init;
    }

    public required string OutputType { get; init; }
}