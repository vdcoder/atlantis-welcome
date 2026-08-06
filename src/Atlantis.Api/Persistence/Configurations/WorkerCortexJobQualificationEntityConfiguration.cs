using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class
    WorkerCortexJobQualificationEntityConfiguration
    : IEntityTypeConfiguration<
        WorkerCortexJobQualificationEntity>
{
    public void Configure(
        EntityTypeBuilder<
            WorkerCortexJobQualificationEntity> builder)
    {
        builder.ToTable(
            "worker_cortex_job_qualifications");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id");

        builder.Property(entity =>
                entity.CortexJobApplicationId)
            .HasColumnName("cortex_job_application_id")
            .IsRequired();

        builder.Property(entity => entity.WorkerCitizenId)
            .HasColumnName("worker_citizen_id")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(entity => entity.DepositAccountId)
            .HasColumnName("deposit_account_id")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(entity =>
                entity.CortexJobDefinitionId)
            .HasColumnName("cortex_job_definition_id")
            .IsRequired();

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(entity => entity.QualifiedAt)
            .HasColumnName("qualified_at")
            .IsRequired();

        builder.Property(entity => entity.SuspendedAt)
            .HasColumnName("suspended_at");

        builder.Property(entity => entity.RevokedAt)
            .HasColumnName("revoked_at");

        builder.HasOne(entity =>
                entity.CortexJobDefinition)
            .WithMany()
            .HasForeignKey(entity =>
                entity.CortexJobDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(entity =>
                entity.CortexJobApplication)
            .WithOne(application =>
                application.WorkerCortexJobQualification)
            .HasForeignKey<WorkerCortexJobQualificationEntity>(
                entity =>
                    entity.CortexJobApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entity =>
                entity.CortexJobApplicationId)
            .IsUnique()
            .HasDatabaseName(
                "ux_worker_cortex_job_qualifications_application");

        builder.HasIndex(entity => new
        {
            entity.WorkerCitizenId,
            entity.CortexJobDefinitionId
        })
            .IsUnique()
            .HasDatabaseName(
                "ux_worker_cortex_job_qualifications_worker_definition");

        builder.HasIndex(entity => entity.DepositAccountId)
            .HasDatabaseName(
                "ix_worker_cortex_job_qualifications_deposit_account");
    }
}