namespace Atlantis.Api.Persistence.Configurations;

using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VisualAttributeDefinitionRecordConfiguration
    : IEntityTypeConfiguration<VisualAttributeDefinitionRecord>
{
    public void Configure(
        EntityTypeBuilder<VisualAttributeDefinitionRecord> builder)
    {
        builder.ToTable(
            "visual_attribute_definitions");

        builder.HasKey(
            record =>
                record.Id);

        builder.Property(
                record =>
                    record.Id)
            .HasColumnName(
                "id")
            .ValueGeneratedNever();

        builder.Property(
                record =>
                    record.Name)
            .HasColumnName(
                "name")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(
                record =>
                    record.Name)
            .IsUnique();
    }
}