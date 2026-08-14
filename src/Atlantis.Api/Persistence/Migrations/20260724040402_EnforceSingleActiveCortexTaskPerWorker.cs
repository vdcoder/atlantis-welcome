using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class EnforceSingleActiveCortexTaskPerWorker : Migration
    {
        /// <inheritdoc />
        protected override void Up(
            MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "worker_citizen_id",
                table: "cortex_task_assignments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql(
                """
        UPDATE cortex_task_assignments AS assignment
        SET worker_citizen_id =
            qualification.worker_citizen_id
        FROM worker_cortex_job_qualifications AS qualification
        WHERE qualification.id =
            assignment.worker_cortex_job_qualification_id;
        """);

            migrationBuilder.Sql(
                """
        DO $$
        BEGIN
            IF EXISTS (
                SELECT 1
                FROM cortex_task_assignments
                WHERE worker_citizen_id IS NULL
                   OR length(btrim(worker_citizen_id)) = 0
            ) THEN
                RAISE EXCEPTION
                    'Unable to backfill worker_citizen_id for all Cortex task assignments.';
            END IF;
        END
        $$;
        """);

            migrationBuilder.AlterColumn<string>(
                name: "worker_citizen_id",
                table: "cortex_task_assignments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.DropIndex(
                name:
                    "ux_cortex_task_assignments_active_qualification",
                table:
                    "cortex_task_assignments");

            migrationBuilder.CreateIndex(
                name:
                    "ux_cortex_task_assignments_active_worker",
                table:
                    "cortex_task_assignments",
                column:
                    "worker_citizen_id",
                unique:
                    true,
                filter:
                    "\"status\" = 1");
        }

        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name:
                    "ux_cortex_task_assignments_active_worker",
                table:
                    "cortex_task_assignments");

            migrationBuilder.CreateIndex(
                name:
                    "ux_cortex_task_assignments_active_qualification",
                table:
                    "cortex_task_assignments",
                column:
                    "worker_cortex_job_qualification_id",
                unique:
                    true,
                filter:
                    "\"status\" = 1");

            migrationBuilder.DropColumn(
                name: "worker_citizen_id",
                table: "cortex_task_assignments");
        }
    }
}
