namespace Atlantis.Api.Citizens.Sponsorship;

/// <summary>
/// Records a support and funding relationship for a citizen.
/// Sponsorship grants the sponsor no authority over the citizen.
/// </summary>
public sealed record CitizenSponsorship
{
    public required Guid Id { get; init; }

    public required string CitizenEntityId { get; init; }

    public required string SponsorEntityId { get; init; }

    public required string FundingAccountId { get; init; }

    public required CitizenSponsorshipStatus Status
    {
        get;
        init;
    }

    public required DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? ActivatedAt { get; init; }

    public DateTimeOffset? EndedAt { get; init; }

    public string? EndReason { get; init; }
}