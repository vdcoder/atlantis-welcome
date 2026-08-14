namespace Atlantis.Api.Persistence.Configurations;

using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class EntityVisualAttributeRecordConfiguration
    : IEntityTypeConfiguration<EntityVisualAttributeRecord>
{
    public void Configure(
        EntityTypeBuilder<EntityVisualAttributeRecord> builder)
    {
        builder.ToTable(
            "entity_visual_attributes",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_entity_visual_attributes_sequence",
                    "\"sequence\" >= 1 AND \"sequence\" <= 5");
            });

        builder.HasKey(
            record =>
                new
                {
                    record.EntityId,
                    record.AttributeValueId
                });

        builder.Property(
                record =>
                    record.EntityId)
            .HasColumnName(
                "entity_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(
                record =>
                    record.AttributeValueId)
            .HasColumnName(
                "attribute_value_id")
            .IsRequired();

        builder.Property(
                record =>
                    record.Sequence)
            .HasColumnName(
                "sequence")
            .IsRequired();

        builder.HasOne(
                record =>
                    record.Entity)
            .WithMany(
                entity =>
                    entity.VisualAttributes)
            .HasForeignKey(
                record =>
                    record.EntityId)
            .OnDelete(
                DeleteBehavior.Cascade);

        builder.HasOne(
                record =>
                    record.AttributeValue)
            .WithMany()
            .HasForeignKey(
                record =>
                    record.AttributeValueId)
            .OnDelete(
                DeleteBehavior.Restrict);

        builder.HasIndex(
                record =>
                    new
                    {
                        record.EntityId,
                        record.Sequence
                    })
            .IsUnique();
    }
}