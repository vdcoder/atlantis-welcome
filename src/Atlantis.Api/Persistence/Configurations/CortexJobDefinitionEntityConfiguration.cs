using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class CortexJobDefinitionEntityConfiguration
    : IEntityTypeConfiguration<CortexJobDefinitionEntity>
{
    public void Configure(
        EntityTypeBuilder<CortexJobDefinitionEntity> builder)
    {
        builder.ToTable("cortex_job_definitions");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id");

        builder.Property(entity => entity.EmployerAccountId)
            .HasColumnName("employer_account_id")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.Description)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(entity => entity.Qualifications)
            .HasColumnName("qualifications")
            .IsRequired();

        builder.Property(entity => entity.CompletionInboxId)
            .HasColumnName("completion_inbox_id")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(entity => entity.FailureInboxId)
            .HasColumnName("failure_inbox_id")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(entity => entity.DeactivatedAt)
            .HasColumnName("deactivated_at");

        builder.HasIndex(entity => new
        {
            entity.EmployerAccountId,
            entity.Name
        })
            .IsUnique()
            .HasDatabaseName(
                "ux_cortex_job_definitions_employer_name");

        builder.HasMany(entity =>
                entity.ScheduleConditions)
            .WithOne(condition =>
                condition.CortexJobDefinition)
            .HasForeignKey(condition =>
                condition.CortexJobDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}