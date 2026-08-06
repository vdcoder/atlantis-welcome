using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class
    SimulateCitizenPassWorkOrderEntityConfiguration
    : IEntityTypeConfiguration<
        SimulateCitizenPassWorkOrderEntity>
{
    public void Configure(
        EntityTypeBuilder<
            SimulateCitizenPassWorkOrderEntity> builder)
    {
        builder.ToTable(
            "simulate_citizen_pass_work_orders");

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
                    entity.ExternalRequestId)
            .HasColumnName(
                "external_request_id")
            .IsRequired();

        builder.Property(
                entity =>
                    entity.DevelopmentWorldId)
            .HasColumnName(
                "development_world_id")
            .HasMaxLength(
                100)
            .IsRequired();

        builder.Property(
                entity =>
                    entity.ScenarioId)
            .HasColumnName(
                "scenario_id")
            .HasMaxLength(
                200)
            .IsRequired();

        builder.Property(
                entity =>
                    entity.SimulatedCitizenId)
            .HasColumnName(
                "simulated_citizen_id")
            .HasMaxLength(
                100)
            .IsRequired();

        builder.Property(
                entity =>
                    entity.SequenceNumber)
            .HasColumnName(
                "sequence_number")
            .IsRequired();

        builder.Property(
                entity =>
                    entity.ContextContractVersion)
            .HasColumnName(
                "context_contract_version")
            .IsRequired();

        builder.Property(
                entity =>
                    entity.ContextHash)
            .HasColumnName(
                "context_hash")
            .HasMaxLength(
                64)
            .IsRequired();

        builder.Property(
                entity =>
                    entity.ContextSerialized)
            .HasColumnName(
                "context_serialized")
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
                    entity.CompletedAt)
            .HasColumnName(
                "completed_at");

        builder.HasOne(
                entity =>
                    entity.SimulateCitizenPassTask)
            .WithOne(
                entity =>
                    entity.WorkOrder)
            .HasForeignKey<
                SimulateCitizenPassWorkOrderEntity>(
                    entity =>
                        entity.CortexTaskId)
            .HasPrincipalKey<
                SimulateCitizenPassTaskEntity>(
                    entity =>
                        entity.CortexTaskId)
            .OnDelete(
                DeleteBehavior.Cascade);

        builder.HasIndex(
                entity =>
                    entity.ExternalRequestId)
            .IsUnique()
            .HasDatabaseName(
                "ux_simulate_citizen_pass_work_orders_external_request");

        builder.HasIndex(
                entity =>
                    entity.CortexTaskId)
            .IsUnique()
            .HasDatabaseName(
                "ux_simulate_citizen_pass_work_orders_cortex_task");

        builder.HasIndex(
                entity =>
                    entity.Status)
            .HasDatabaseName(
                "ix_simulate_citizen_pass_work_orders_status");
    }
}