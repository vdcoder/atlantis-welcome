namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation.Contracts;

public sealed record SimulationDirectionDto
{
    public required float X { get; init; }

    public required float Y { get; init; }

    public required float Z { get; init; }
}