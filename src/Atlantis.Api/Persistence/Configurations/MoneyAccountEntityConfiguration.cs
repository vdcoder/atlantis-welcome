using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class MoneyAccountEntityConfiguration
    : IEntityTypeConfiguration<MoneyAccountEntity>
{
    public void Configure(
        EntityTypeBuilder<MoneyAccountEntity> builder)
    {
        builder.ToTable("money_accounts");

        builder.HasKey(account => account.Id);

        builder.Property(account => account.Id)
            .HasColumnName("id")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(account => account.OwnerId)
            .HasColumnName("owner_id")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(account => account.OwnerType)
            .HasColumnName("owner_type")
            .IsRequired();

        builder.Property(account => account.AccountType)
            .HasColumnName("account_type")
            .IsRequired();

        builder.Property(account => account.Currency)
            .HasColumnName("currency")
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(account => account.Name)
            .HasColumnName("name")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(account => account.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(account => account.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.HasIndex(account => account.OwnerId)
            .HasDatabaseName("ix_money_accounts_owner_id");

        builder.HasIndex(account => new
        {
            account.OwnerId,
            account.AccountType,
            account.Currency
        })
            .IsUnique()
            .HasDatabaseName(
                "ux_money_accounts_owner_account_type_currency");
    }
}