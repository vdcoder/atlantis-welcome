namespace Atlantis.Api.Citizens.Brain.CortexJobs.Qualifications;

public sealed record WorkerCortexJobQualification
{
    public required Guid Id { get; init; }

    public required Guid CortexJobApplicationId { get; init; }

    public required string WorkerCitizenId { get; init; }

    public required string DepositAccountId { get; init; }

    public required Guid CortexJobDefinitionId { get; init; }

    public required WorkerCortexJobQualificationStatus Status
    {
        get;
        init;
    }

    public required DateTimeOffset QualifiedAt { get; init; }

    public DateTimeOffset? SuspendedAt { get; init; }

    public DateTimeOffset? RevokedAt { get; init; }
}