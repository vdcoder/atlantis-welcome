using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class CortexJobInboxMessageRecordConfiguration
    : IEntityTypeConfiguration<CortexJobInboxMessageRecord>
{
    public void Configure(
        EntityTypeBuilder<CortexJobInboxMessageRecord> builder)
    {
        builder.ToTable(
            "cortex_job_inbox_messages");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id");

        builder.Property(entity => entity.InboxId)
            .HasColumnName("inbox_id")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(entity => entity.CortexTaskId)
            .HasColumnName("cortex_task_id")
            .IsRequired();

        builder.Property(entity =>
                entity.CortexTaskAssignmentId)
            .HasColumnName(
                "cortex_task_assignment_id")
            .IsRequired();

        builder.Property(entity => entity.Type)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(entity => entity.PayloadType)
            .HasColumnName("payload_type")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(entity =>
                entity.PayloadSerialized)
            .HasColumnName("payload_serialized")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(entity => entity.ProcessedAt)
            .HasColumnName("processed_at");

        builder.HasOne(entity => entity.CortexTask)
            .WithMany()
            .HasForeignKey(entity => entity.CortexTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(entity => entity.Assignment)
            .WithMany()
            .HasForeignKey(entity =>
                entity.CortexTaskAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entity => new
        {
            entity.InboxId,
            entity.ProcessedAt,
            entity.CreatedAt
        })
            .HasDatabaseName(
                "ix_cortex_job_inbox_messages_delivery");

        builder.HasIndex(entity => new
        {
            entity.CortexTaskAssignmentId,
            entity.Type
        })
            .IsUnique()
            .HasDatabaseName(
                "ux_cortex_job_inbox_messages_assignment_type");
    }
}