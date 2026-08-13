using Atlantis.Api.Citizens.Sponsorship;
using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class CitizenSponsorshipRecordConfiguration
    : IEntityTypeConfiguration<CitizenSponsorshipRecord>
{
    public void Configure(
        EntityTypeBuilder<CitizenSponsorshipRecord> builder)
    {
        builder.ToTable(
            "citizen_sponsorships",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "ck_citizen_sponsorships_distinct_entities",
                    "\"citizen_entity_id\" <> \"sponsor_entity_id\"");

                tableBuilder.HasCheckConstraint(
                    "ck_citizen_sponsorships_end_reason",
                    "\"end_reason\" IS NULL OR " +
                    "length(btrim(\"end_reason\")) > 0");

                tableBuilder.HasCheckConstraint(
                    "ck_citizen_sponsorships_status_dates",
                    """
                    (
                        "status" = 1
                        AND "activated_at" IS NULL
                        AND "ended_at" IS NULL
                    )
                    OR
                    (
                        "status" = 2
                        AND "activated_at" IS NOT NULL
                        AND "activated_at" >= "created_at"
                        AND "ended_at" IS NULL
                    )
                    OR
                    (
                        "status" IN (3, 4)
                        AND "activated_at" IS NOT NULL
                        AND "activated_at" >= "created_at"
                        AND "ended_at" IS NOT NULL
                        AND "ended_at" >= "activated_at"
                    )
    """);

                tableBuilder.HasCheckConstraint(
                    "ck_citizen_sponsorships_status",
                    "\"status\" IN (1, 2, 3, 4)");
            });

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id");

        builder.Property(entity => entity.CitizenEntityId)
            .HasColumnName("citizen_entity_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity => entity.SponsorEntityId)
            .HasColumnName("sponsor_entity_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity => entity.FundingAccountId)
            .HasColumnName("funding_account_id")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(entity => entity.ActivatedAt)
            .HasColumnName("activated_at");

        builder.Property(entity => entity.EndedAt)
            .HasColumnName("ended_at");

        builder.Property(entity => entity.EndReason)
            .HasColumnName("end_reason")
            .HasMaxLength(2000);

        builder.HasOne(entity => entity.FundingAccount)
            .WithMany()
            .HasForeignKey(entity => entity.FundingAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entity => entity.CitizenEntityId)
            .IsUnique()
            .HasFilter(
                $"\"status\" = " +
                $"{(int)CitizenSponsorshipStatus.Active}")
            .HasDatabaseName(
                "ux_citizen_sponsorships_active_citizen");

        builder.HasIndex(entity => entity.SponsorEntityId)
            .HasDatabaseName(
                "ix_citizen_sponsorships_sponsor");

        builder.HasIndex(entity => entity.FundingAccountId)
            .HasDatabaseName(
                "ix_citizen_sponsorships_funding_account");
    }
}