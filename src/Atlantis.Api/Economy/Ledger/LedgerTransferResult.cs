namespace Atlantis.Api.Economy.Ledger;

public sealed record LedgerTransferResult
{
    public required Guid TransactionId { get; init; }

    public required LedgerEntry DebitEntry { get; init; }

    public required LedgerEntry CreditEntry { get; init; }

    public required decimal Amount { get; init; }

    public required string Currency { get; init; }
}