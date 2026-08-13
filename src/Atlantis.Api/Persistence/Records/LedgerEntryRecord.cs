namespace Atlantis.Api.Persistence.Records;

public sealed class LedgerEntryRecord
{
    public Guid Id { get; set; }

    public Guid TransactionId { get; set; }

    public string AccountId { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public string? ReferenceType { get; set; }

    public string? ReferenceId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public MoneyAccountRecord Account { get; set; } = null!;
}