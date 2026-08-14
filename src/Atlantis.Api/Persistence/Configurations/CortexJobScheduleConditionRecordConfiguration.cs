using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class CortexJobScheduleConditionRecordConfiguration
    : IEntityTypeConfiguration<CortexJobScheduleConditionRecord>
{
    public void Configure(
        EntityTypeBuilder<CortexJobScheduleConditionRecord> builder)
    {
        builder.ToTable(
            "cortex_job_schedule_conditions");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id");

        builder.Property(entity =>
                entity.CortexJobDefinitionId)
            .HasColumnName("cortex_job_definition_id")
            .IsRequired();

        builder.Property(entity => entity.ConditionType)
            .HasColumnName("condition_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity =>
                entity.ConfigurationJson)
            .HasColumnName("configuration_json")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(entity => entity.SortOrder)
            .HasColumnName("sort_order")
            .IsRequired();

        builder.HasIndex(entity => new
        {
            entity.CortexJobDefinitionId,
            entity.SortOrder
        })
            .IsUnique()
            .HasDatabaseName(
                "ux_cortex_job_schedule_conditions_definition_order");

        builder.HasIndex(entity => entity.ConditionType)
            .HasDatabaseName(
                "ix_cortex_job_schedule_conditions_type");
    }
}