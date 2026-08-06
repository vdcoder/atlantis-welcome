namespace Atlantis.Api.Citizens.Brain.CortexJobs.Availability;

public sealed record WorkerCortexJobAvailability
{
    public required Guid WorkerCortexJobQualificationId { get; init; }

    public required bool IsAvailable { get; init; }

    public required DateTimeOffset ChangedAt { get; init; }
}