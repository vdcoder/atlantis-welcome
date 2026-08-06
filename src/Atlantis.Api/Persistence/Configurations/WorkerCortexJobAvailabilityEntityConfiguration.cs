using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class
    WorkerCortexJobAvailabilityEntityConfiguration
    : IEntityTypeConfiguration<
        WorkerCortexJobAvailabilityEntity>
{
    public void Configure(
        EntityTypeBuilder<
            WorkerCortexJobAvailabilityEntity> builder)
    {
        builder.ToTable(
            "worker_cortex_job_availability");

        builder.HasKey(entity =>
            entity.WorkerCortexJobQualificationId);

        builder.Property(entity =>
                entity.WorkerCortexJobQualificationId)
            .HasColumnName(
                "worker_cortex_job_qualification_id");

        builder.Property(entity => entity.IsAvailable)
            .HasColumnName("is_available")
            .IsRequired();

        builder.Property(entity => entity.ChangedAt)
            .HasColumnName("changed_at")
            .IsRequired();

        builder.HasOne(entity => entity.Qualification)
            .WithOne(entity => entity.Availability)
            .HasForeignKey<
                WorkerCortexJobAvailabilityEntity>(
                entity =>
                    entity.WorkerCortexJobQualificationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(entity => entity.IsAvailable)
            .HasDatabaseName(
                "ix_worker_cortex_job_availability_is_available");
    }
}