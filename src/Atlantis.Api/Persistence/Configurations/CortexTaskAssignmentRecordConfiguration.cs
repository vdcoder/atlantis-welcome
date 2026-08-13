using Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;
using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlantis.Api.Persistence.Configurations;

public sealed class CortexTaskAssignmentRecordConfiguration
    : IEntityTypeConfiguration<CortexTaskAssignmentRecord>
{
    public void Configure(
        EntityTypeBuilder<CortexTaskAssignmentRecord> builder)
    {
        builder.ToTable(
            "cortex_task_assignments",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "ck_cortex_task_assignments_assignment_priming_times",
                    "\"first_primed_at\" >= \"assigned_at\" AND " +
                    "\"last_primed_at\" >= \"first_primed_at\"");

                tableBuilder.HasCheckConstraint(
                    "ck_cortex_task_assignments_failure_reason",
                    "\"failure_reason\" IS NULL OR " +
                    "length(btrim(\"failure_reason\")) > 0");
            });

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id");

        builder.Property(entity => entity.CortexTaskId)
            .HasColumnName("cortex_task_id")
            .IsRequired();

        builder.Property(entity =>
                entity.WorkerCortexJobQualificationId)
            .HasColumnName(
                "worker_cortex_job_qualification_id")
            .IsRequired();

        builder.Property(entity => entity.WorkerCitizenId)
            .HasColumnName("worker_citizen_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(entity => entity.AssignedAt)
            .HasColumnName("assigned_at")
            .IsRequired();

        builder.Property(entity => entity.FirstPrimedAt)
            .HasColumnName("first_primed_at")
            .IsRequired();

        builder.Property(entity => entity.LastPrimedAt)
            .HasColumnName("last_primed_at")
            .IsRequired();

        builder.Property(entity => entity.CompletedAt)
            .HasColumnName("completed_at");

        builder.Property(entity => entity.FailedAt)
            .HasColumnName("failed_at");

        builder.Property(entity => entity.CancelledAt)
            .HasColumnName("cancelled_at");

        builder.Property(entity => entity.PaidAt)
            .HasColumnName("paid_at");

        builder.Property(entity => entity.FailureReason)
            .HasColumnName("failure_reason")
            .HasMaxLength(2000);

        builder.Property(entity =>
                entity.PaymentTransactionId)
            .HasColumnName("payment_transaction_id");

        builder.HasOne(entity => entity.CortexTask)
            .WithOne(task => task.Assignment)
            .HasForeignKey<CortexTaskAssignmentRecord>(
                entity => entity.CortexTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(entity =>
                entity.WorkerCortexJobQualification)
            .WithMany()
            .HasForeignKey(entity =>
                entity.WorkerCortexJobQualificationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entity => entity.CortexTaskId)
            .IsUnique()
            .HasDatabaseName(
                "ux_cortex_task_assignments_task");

        builder.HasIndex(entity => new
        {
            entity.WorkerCortexJobQualificationId,
            entity.Status
        })
            .HasDatabaseName(
                "ix_cortex_task_assignments_qualification_status");

        builder.HasIndex(entity => entity.AssignedAt)
            .HasDatabaseName(
                "ix_cortex_task_assignments_assigned_at");

        builder.HasIndex(entity =>
                entity.PaymentTransactionId)
            .HasFilter(
                "\"payment_transaction_id\" IS NOT NULL")
            .HasDatabaseName(
                "ix_cortex_task_assignments_payment_transaction");

        builder.HasIndex(entity =>
                entity.WorkerCitizenId)
            .IsUnique()
            .HasFilter(
                $"\"status\" = " +
                $"{(int)CortexTaskAssignmentStatus.Primed}")
            .HasDatabaseName(
                "ux_cortex_task_assignments_active_worker");
    }
}