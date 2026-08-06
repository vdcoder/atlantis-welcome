using Atlantis.Api.Development.Predictions.Contracts;

namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation.WorkOrders;

public sealed record
    SimulateCitizenPassWorkOrderCompletion
{
    public required string WorkerCitizenId
    {
        get;
        init;
    }

    public required DateTimeOffset RecordedAt
    {
        get;
        init;
    }

    public required RecordedCitizenBreathDto
        RecordedBreath
    {
        get;
        init;
    }
}