namespace Atlantis.Api.Economy.Contracts;

public sealed record TransferRequest
{
    public required string FromAccountId { get; init; }

    public required string ToAccountId { get; init; }

    public required decimal Amount { get; init; }

    public required string Reason { get; init; }

    public string? ReferenceType { get; init; }

    public string? ReferenceId { get; init; }
}