using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class
    DevelopmentPredictionRequestRecordConfiguration
    : IEntityTypeConfiguration<
        DevelopmentPredictionRequestRecord>
{
    public void Configure(
        EntityTypeBuilder<
            DevelopmentPredictionRequestRecord> builder)
    {
        builder.ToTable(
            "development_prediction_requests");

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
                    entity.ContextSerialized)
            .HasColumnName(
                "context_serialized")
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
                    entity.Status)
            .HasColumnName(
                "status")
            .IsRequired();

        builder.Property(
                entity =>
                    entity.ContextContractVersion)
            .HasColumnName(
                "context_contract_version")
            .IsRequired();

        builder.Property(
                entity =>
                    entity.CreatedAt)
            .HasColumnName(
                "created_at")
            .IsRequired();

        builder.HasIndex(
                entity =>
                    new
                    {
                        entity.DevelopmentWorldId,
                        entity.ScenarioId,
                        entity.SimulatedCitizenId,
                        entity.SequenceNumber,
                        entity.ContextContractVersion,
                        entity.ContextHash
                    })
            .IsUnique()
            .HasDatabaseName(
                "ux_development_prediction_requests_exact_input");
    }
}