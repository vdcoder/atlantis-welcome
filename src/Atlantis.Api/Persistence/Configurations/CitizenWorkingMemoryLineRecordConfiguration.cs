using Atlantis.Api.Persistence.Records;
using Atlantis.Api.Citizens.Brain.Memory.WorkingMemory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations
{
    public sealed class CitizenWorkingMemoryLineRecordConfiguration
        : IEntityTypeConfiguration<CitizenWorkingMemoryLineRecord>
    {
        public void Configure(
            EntityTypeBuilder<CitizenWorkingMemoryLineRecord> builder)
        {
            builder.ToTable(
                "citizen_working_memory_lines");

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
                        record.LineNumber)
                .HasColumnName(
                    "line_number")
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
                            record.LineNumber,
                            record.CreatedAt,
                            record.Id
                        })
                .IsDescending(
                    false,
                    false,
                    true,
                    true);

            builder.ToTable(
                table =>
                {
                    table.HasCheckConstraint(
                        "ck_citizen_working_memory_line_number",
                        $"line_number >= 1 AND " +
                        $"line_number <= {WorkingMemoryLimits.MaxLines}");

                    table.HasCheckConstraint(
                        "ck_citizen_working_memory_line_token_count",
                        $"content_token_count >= 0 AND " +
                        $"content_token_count <= {WorkingMemoryLimits.MaxLineTokens}");
                });
        }
    }
}
