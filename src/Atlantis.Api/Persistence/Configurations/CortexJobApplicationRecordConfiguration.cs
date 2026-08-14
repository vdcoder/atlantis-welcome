using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class CortexJobApplicationRecordConfiguration
    : IEntityTypeConfiguration<CortexJobApplicationRecord>
{
    public void Configure(
        EntityTypeBuilder<CortexJobApplicationRecord> builder)
    {
        builder.ToTable("cortex_job_applications");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id");

        builder.Property(entity => entity.CortexJobDefinitionId)
            .HasColumnName("cortex_job_definition_id")
            .IsRequired();

        builder.Property(entity => entity.WorkerCitizenId)
            .HasColumnName("worker_citizen_id")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(entity => entity.DepositAccountId)
            .HasColumnName("deposit_account_id")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(entity => entity.AppliedAt)
            .HasColumnName("applied_at")
            .IsRequired();

        builder.Property(entity => entity.ApprovedAt)
            .HasColumnName("approved_at");

        builder.Property(entity => entity.RejectedAt)
            .HasColumnName("rejected_at");

        builder.Property(entity => entity.RejectionReason)
            .HasColumnName("rejection_reason")
            .HasMaxLength(1000);

        builder.HasOne(entity =>
                entity.CortexJobDefinition)
            .WithMany()
            .HasForeignKey(entity =>
                entity.CortexJobDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entity => new
        {
            entity.WorkerCitizenId,
            entity.CortexJobDefinitionId
        })
            .HasDatabaseName(
                "ix_cortex_job_applications_worker_definition");
    }
}