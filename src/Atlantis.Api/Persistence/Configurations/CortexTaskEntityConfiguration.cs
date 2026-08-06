using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class CortexTaskEntityConfiguration
    : IEntityTypeConfiguration<CortexTaskEntity>
{
    public void Configure(
        EntityTypeBuilder<CortexTaskEntity> builder)
    {
        builder.ToTable(
            "cortex_tasks",
            tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "ck_cortex_tasks_reward_positive",
                    "\"reward\" > 0"));

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id");

        builder.Property(entity =>
                entity.CortexJobDefinitionId)
            .HasColumnName("cortex_job_definition_id")
            .IsRequired();

        builder.Property(entity => entity.Instructions)
            .HasColumnName("instructions")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(entity => entity.Reward)
            .HasColumnName("reward")
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(entity => entity.Currency)
            .HasColumnName("currency")
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(entity =>
                entity.CompletionInboxId)
            .HasColumnName("completion_inbox_id")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(entity =>
                entity.FailureInboxId)
            .HasColumnName("failure_inbox_id")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(entity => entity.AvailableAt)
            .HasColumnName("available_at");

        builder.Property(entity => entity.UnavailableAt)
            .HasColumnName("unavailable_at");

        builder.HasOne(entity =>
                entity.CortexJobDefinition)
            .WithMany()
            .HasForeignKey(entity =>
                entity.CortexJobDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entity => entity.Status)
            .HasDatabaseName(
                "ix_cortex_tasks_status");

        builder.HasIndex(entity => new
        {
            entity.CortexJobDefinitionId,
            entity.Status,
            entity.AvailableAt
        })
            .HasDatabaseName(
                "ix_cortex_tasks_definition_status_available");

        builder.HasIndex(entity => entity.CreatedAt)
            .HasDatabaseName(
                "ix_cortex_tasks_created_at");
    }
}