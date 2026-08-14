namespace Atlantis.Api.Persistence.Records;

public sealed class CitizenSponsorshipRecord
{
    public Guid Id { get; set; }

    public required string CitizenEntityId { get; set; }

    public required string SponsorEntityId { get; set; }

    public required string FundingAccountId { get; set; }

    public int Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ActivatedAt { get; set; }

    public DateTimeOffset? EndedAt { get; set; }

    public string? EndReason { get; set; }

    public MoneyAccountRecord FundingAccount
    {
        get;
        set;
    } = null!;
}