namespace Atlantis.Api.Persistence.Entities;

public sealed class MoneyAccountEntity
{
    public string Id { get; set; } = null!;

    public string OwnerId { get; set; } = null!;

    public int OwnerType { get; set; }

    public int AccountType { get; set; }

    public string Currency { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public bool IsActive { get; set; }
}