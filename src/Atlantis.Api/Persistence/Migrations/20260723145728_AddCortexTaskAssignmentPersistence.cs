using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCortexTaskAssignmentPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cortex_task_assignments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cortex_task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_cortex_job_qualification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    assigned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    first_primed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_primed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    failed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    paid_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    failure_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    payment_transaction_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cortex_task_assignments", x => x.id);
                    table.CheckConstraint("ck_cortex_task_assignments_assignment_priming_times", "\"first_primed_at\" >= \"assigned_at\" AND \"last_primed_at\" >= \"first_primed_at\"");
                    table.CheckConstraint("ck_cortex_task_assignments_failure_reason", "\"failure_reason\" IS NULL OR length(btrim(\"failure_reason\")) > 0");
                    table.ForeignKey(
                        name: "FK_cortex_task_assignments_cortex_tasks_cortex_task_id",
                        column: x => x.cortex_task_id,
                        principalTable: "cortex_tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cortex_task_assignments_worker_cortex_job_qualifications_wo~",
                        column: x => x.worker_cortex_job_qualification_id,
                        principalTable: "worker_cortex_job_qualifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cortex_task_assignments_assigned_at",
                table: "cortex_task_assignments",
                column: "assigned_at");

            migrationBuilder.CreateIndex(
                name: "ix_cortex_task_assignments_payment_transaction",
                table: "cortex_task_assignments",
                column: "payment_transaction_id",
                filter: "\"payment_transaction_id\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_cortex_task_assignments_qualification_status",
                table: "cortex_task_assignments",
                columns: new[] { "worker_cortex_job_qualification_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ux_cortex_task_assignments_active_qualification",
                table: "cortex_task_assignments",
                column: "worker_cortex_job_qualification_id",
                unique: true,
                filter: "\"status\" = 1");

            migrationBuilder.CreateIndex(
                name: "ux_cortex_task_assignments_task",
                table: "cortex_task_assignments",
                column: "cortex_task_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cortex_task_assignments");
        }
    }
}
