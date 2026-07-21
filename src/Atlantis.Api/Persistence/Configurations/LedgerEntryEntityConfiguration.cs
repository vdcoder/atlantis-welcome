using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class LedgerEntryEntityConfiguration
    : IEntityTypeConfiguration<LedgerEntryEntity>
{
    public void Configure(
        EntityTypeBuilder<LedgerEntryEntity> builder)
    {
        builder.ToTable("ledger_entries");

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(entry => entry.TransactionId)
            .HasColumnName("transaction_id")
            .IsRequired();

        builder.Property(entry => entry.AccountId)
            .HasColumnName("account_id")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(entry => entry.Amount)
            .HasColumnName("amount")
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(entry => entry.Currency)
            .HasColumnName("currency")
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(entry => entry.Reason)
            .HasColumnName("reason")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(entry => entry.ReferenceType)
            .HasColumnName("reference_type")
            .HasMaxLength(128);

        builder.Property(entry => entry.ReferenceId)
            .HasColumnName("reference_id")
            .HasMaxLength(128);

        builder.Property(entry => entry.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne(entry => entry.Account)
            .WithMany()
            .HasForeignKey(entry => entry.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entry => entry.AccountId)
            .HasDatabaseName("ix_ledger_entries_account_id");

        builder.HasIndex(entry => new
        {
            entry.AccountId,
            entry.CreatedAt
        })
            .HasDatabaseName(
                "ix_ledger_entries_account_created_at");

        builder.HasIndex(entry => new
        {
            entry.ReferenceType,
            entry.ReferenceId
        })
            .HasDatabaseName(
                "ix_ledger_entries_reference");

        builder.HasIndex(entry => new
        {
            entry.TransactionId,
            entry.AccountId
        })
            .IsUnique()
            .HasDatabaseName(
                "ux_ledger_entries_transaction_id_account_id");
    }
}