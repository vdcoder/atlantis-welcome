using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class
    CortexTaskRemunerationEntityConfiguration
    : IEntityTypeConfiguration<
        CortexTaskRemunerationEntity>
{
    public void Configure(
        EntityTypeBuilder<
            CortexTaskRemunerationEntity> builder)
    {
        builder.ToTable(
            "cortex_task_remunerations");

        builder.HasKey(
            entity =>
                entity.Id);

        builder.Property(
                entity =>
                    entity.Id)
            .HasColumnName(
                "id")
            .IsRequired();

        builder.Property(
                entity =>
                    entity.CortexTaskId)
            .HasColumnName(
                "cortex_task_id")
            .IsRequired();

        builder.Property(
                entity =>
                    entity.CortexTaskAssignmentId)
            .HasColumnName(
                "cortex_task_assignment_id")
            .IsRequired();

        builder.Property(
                entity =>
                    entity.WorkerCitizenId)
            .HasColumnName(
                "worker_citizen_id")
            .HasMaxLength(
                200)
            .IsRequired();

        builder.Property(
                entity =>
                    entity.EmployerAccountId)
            .HasColumnName(
                "employer_account_id")
            .HasMaxLength(
                200)
            .IsRequired();

        builder.Property(
                entity =>
                    entity.Amount)
            .HasColumnName(
                "amount")
            .HasPrecision(
                18,
                2)
            .IsRequired();

        builder.Property(
                entity =>
                    entity.Currency)
            .HasColumnName(
                "currency")
            .HasMaxLength(
                20)
            .IsRequired();

        builder.Property(
                entity =>
                    entity.Status)
            .HasColumnName(
                "status")
            .IsRequired();

        builder.Property(
                entity =>
                    entity.CreatedAt)
            .HasColumnName(
                "created_at")
            .IsRequired();

        builder.Property(
                entity =>
                    entity.PaidAt)
            .HasColumnName(
                "paid_at");

        builder.Property(
                entity =>
                    entity.LedgerTransactionId)
            .HasColumnName(
                "ledger_transaction_id");

        builder.HasOne<
                CortexTaskEntity>()
            .WithMany()
            .HasForeignKey(
                entity =>
                    entity.CortexTaskId)
            .OnDelete(
                DeleteBehavior.Restrict);

        builder.HasOne<
                CortexTaskAssignmentEntity>()
            .WithMany()
            .HasForeignKey(
                entity =>
                    entity.CortexTaskAssignmentId)
            .OnDelete(
                DeleteBehavior.Restrict);

        builder.HasIndex(
                entity =>
                    entity.CortexTaskId)
            .IsUnique()
            .HasDatabaseName(
                "ux_cortex_task_remunerations_task");

        builder.HasIndex(
                entity =>
                    entity.CortexTaskAssignmentId)
            .IsUnique()
            .HasDatabaseName(
                "ux_cortex_task_remunerations_assignment");

        builder.HasIndex(
                entity =>
                    entity.LedgerTransactionId)
            .IsUnique()
            .HasFilter(
                "ledger_transaction_id IS NOT NULL")
            .HasDatabaseName(
                "ux_cortex_task_remunerations_ledger_transaction");

        builder.HasIndex(
                entity =>
                    new
                    {
                        entity.Status,
                        entity.CreatedAt
                    })
            .HasDatabaseName(
                "ix_cortex_task_remunerations_pending_order");
    }
}