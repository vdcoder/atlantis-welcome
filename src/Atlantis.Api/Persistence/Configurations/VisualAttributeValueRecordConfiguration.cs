namespace Atlantis.Api.Persistence.Configurations;

using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VisualAttributeValueRecordConfiguration
    : IEntityTypeConfiguration<VisualAttributeValueRecord>
{
    public void Configure(
        EntityTypeBuilder<VisualAttributeValueRecord> builder)
    {
        builder.ToTable(
            "visual_attribute_values");

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
                    record.AttributeDefinitionId)
            .HasColumnName(
                "attribute_definition_id")
            .IsRequired();

        builder.Property(
                record =>
                    record.Value)
            .HasColumnName(
                "value")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(
                record =>
                    record.DisplayText)
            .HasColumnName(
                "display_text")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(
                record =>
                    record.AttributeDefinition)
            .WithMany()
            .HasForeignKey(
                record =>
                    record.AttributeDefinitionId)
            .OnDelete(
                DeleteBehavior.Restrict);

        builder.HasIndex(
                record =>
                    new
                    {
                        record.AttributeDefinitionId,
                        record.Value
                    })
            .IsUnique();
    }
}