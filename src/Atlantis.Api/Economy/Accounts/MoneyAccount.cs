namespace Atlantis.Api.Economy.Accounts;

public sealed record MoneyAccount
{
    public required string Id { get; init; }

    public required string OwnerId { get; init; }

    public required AccountOwnerType OwnerType { get; init; }

    public required AccountType AccountType { get; init; }

    public required string Currency { get; init; }

    public required string Name { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public bool IsActive { get; init; } = true;
}