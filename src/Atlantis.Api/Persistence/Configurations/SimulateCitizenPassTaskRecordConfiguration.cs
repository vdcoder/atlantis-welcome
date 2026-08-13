using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class
    SimulateCitizenPassTaskRecordConfiguration
    : IEntityTypeConfiguration<
        SimulateCitizenPassTaskRecord>
{
    public void Configure(
        EntityTypeBuilder<
            SimulateCitizenPassTaskRecord> builder)
    {
        builder.ToTable(
            "simulate_citizen_pass_tasks");

        builder.HasKey(entity =>
            entity.CortexTaskId);

        builder.Property(entity =>
                entity.CortexTaskId)
            .HasColumnName("cortex_task_id");

        builder.Property(entity =>
                entity.ExternalCitizenId)
            .HasColumnName("external_citizen_id")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity =>
                entity.ExternalWorldId)
            .HasColumnName("external_world_id")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity =>
                entity.ExternalRequestId)
            .HasColumnName("external_request_id")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity =>
                entity.ExternalScenarioId)
            .HasColumnName("external_scenario_id")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.ReceivedAt)
            .HasColumnName("received_at")
            .IsRequired();

        builder.HasOne(entity => entity.CortexTask)
            .WithOne(entity =>
                entity.SimulateCitizenPassTask)
            .HasForeignKey<
                SimulateCitizenPassTaskRecord>(
                entity => entity.CortexTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(entity => new
        {
            entity.ExternalWorldId,
            entity.ExternalRequestId
        })
            .IsUnique()
            .HasDatabaseName(
                "ux_simulate_citizen_pass_tasks_world_request");

        builder.HasIndex(entity =>
                entity.ExternalCitizenId)
            .HasDatabaseName(
                "ix_simulate_citizen_pass_tasks_external_citizen");

        builder.HasIndex(entity =>
                entity.ExternalScenarioId)
            .HasDatabaseName(
                "ix_simulate_citizen_pass_tasks_external_scenario");
    }
}