using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCortexTaskPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cortex_tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cortex_job_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    instructions = table.Column<string>(type: "jsonb", nullable: false),
                    reward = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    completion_inbox_id = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    failure_inbox_id = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    available_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    unavailable_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cortex_tasks", x => x.id);
                    table.CheckConstraint("ck_cortex_tasks_reward_positive", "\"reward\" > 0");
                    table.ForeignKey(
                        name: "FK_cortex_tasks_cortex_job_definitions_cortex_job_definition_id",
                        column: x => x.cortex_job_definition_id,
                        principalTable: "cortex_job_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "simulate_citizen_pass_tasks",
                columns: table => new
                {
                    cortex_task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_citizen_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    external_world_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    external_request_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    external_scenario_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_simulate_citizen_pass_tasks", x => x.cortex_task_id);
                    table.ForeignKey(
                        name: "FK_simulate_citizen_pass_tasks_cortex_tasks_cortex_task_id",
                        column: x => x.cortex_task_id,
                        principalTable: "cortex_tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cortex_tasks_created_at",
                table: "cortex_tasks",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_cortex_tasks_definition_status_available",
                table: "cortex_tasks",
                columns: new[] { "cortex_job_definition_id", "status", "available_at" });

            migrationBuilder.CreateIndex(
                name: "ix_cortex_tasks_status",
                table: "cortex_tasks",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_simulate_citizen_pass_tasks_external_citizen",
                table: "simulate_citizen_pass_tasks",
                column: "external_citizen_id");

            migrationBuilder.CreateIndex(
                name: "ix_simulate_citizen_pass_tasks_external_scenario",
                table: "simulate_citizen_pass_tasks",
                column: "external_scenario_id");

            migrationBuilder.CreateIndex(
                name: "ux_simulate_citizen_pass_tasks_world_request",
                table: "simulate_citizen_pass_tasks",
                columns: new[] { "external_world_id", "external_request_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "simulate_citizen_pass_tasks");

            migrationBuilder.DropTable(
                name: "cortex_tasks");
        }
    }
}
