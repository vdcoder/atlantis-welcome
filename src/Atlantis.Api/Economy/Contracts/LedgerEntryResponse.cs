namespace Atlantis.Api.Economy.Contracts;

public sealed record LedgerEntryResponse
{
    public required Guid Id { get; init; }

    public required Guid TransactionId { get; init; }

    public required string AccountId { get; init; }

    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required string Reason { get; init; }

    public string? ReferenceType { get; init; }

    public string? ReferenceId { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
}