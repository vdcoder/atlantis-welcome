namespace Atlantis.Api.Economy.Contracts;

public sealed record AccountBalanceResponse
{
    public required string AccountId { get; init; }

    public required string AccountName { get; init; }

    public required decimal Balance { get; init; }

    public required string Currency { get; init; }

    public required bool IsActive { get; init; }
}