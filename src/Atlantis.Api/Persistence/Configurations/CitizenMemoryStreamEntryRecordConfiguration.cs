using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations
{
    public sealed class CitizenMemoryStreamEntryRecordConfiguration
        : IEntityTypeConfiguration<CitizenMemoryStreamEntryRecord>
    {
        public void Configure(
            EntityTypeBuilder<CitizenMemoryStreamEntryRecord> builder)
        {
            builder.ToTable(
                "citizen_memory_stream_entries");

            builder.Property(
                    record =>
                        record.Id)
                .HasColumnName(
                    "id");

            builder.HasKey(
                record =>
                    record.Id);

            builder.Property(
                    record =>
                        record.CitizenId)
                .HasColumnName(
                    "citizen_id")
                .IsRequired();

            builder.Property(
                    record =>
                        record.Content)
                .HasColumnName(
                    "content")
                .IsRequired();

            builder.Property(
                    record =>
                        record.ContentTokenCount)
                .HasColumnName(
                    "content_token_count")
                .IsRequired();

            builder.Property(
                    record =>
                        record.CreatedAt)
                .HasColumnName(
                    "created_at")
                .IsRequired();

            builder.HasIndex(
                    record =>
                        new
                        {
                            record.CitizenId,
                            record.CreatedAt,
                            record.Id
                        })
                .IsDescending(
                    false,
                    true,
                    true);

            builder.ToTable(
                table =>
                {
                    table.HasCheckConstraint(
                        "ck_citizen_memory_stream_entry_token_count",
                        "content_token_count > 0");
                });
        }
    }
}