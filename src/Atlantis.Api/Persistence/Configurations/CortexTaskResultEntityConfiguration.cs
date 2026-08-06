using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class CortexTaskResultEntityConfiguration
    : IEntityTypeConfiguration<CortexTaskResultEntity>
{
    public void Configure(
        EntityTypeBuilder<CortexTaskResultEntity> builder)
    {
        builder.ToTable("cortex_task_results");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id");

        builder.Property(entity =>
                entity.CortexTaskAssignmentId)
            .HasColumnName(
                "cortex_task_assignment_id")
            .IsRequired();

        builder.Property(entity => entity.ResultType)
            .HasColumnName("result_type")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(entity =>
                entity.ResultSerialized)
            .HasColumnName("result_serialized")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(entity => entity.CompletedAt)
            .HasColumnName("completed_at")
            .IsRequired();

        builder.HasOne(entity => entity.Assignment)
            .WithOne(assignment => assignment.Result)
            .HasForeignKey<CortexTaskResultEntity>(
                entity =>
                    entity.CortexTaskAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entity =>
                entity.CortexTaskAssignmentId)
            .IsUnique()
            .HasDatabaseName(
                "ux_cortex_task_results_assignment");
    }
}