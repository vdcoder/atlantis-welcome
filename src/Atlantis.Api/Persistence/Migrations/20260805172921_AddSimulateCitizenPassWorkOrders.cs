using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSimulateCitizenPassWorkOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "simulate_citizen_pass_work_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cortex_task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    development_world_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    scenario_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    simulated_citizen_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sequence_number = table.Column<long>(type: "bigint", nullable: false),
                    context_contract_version = table.Column<int>(type: "integer", nullable: false),
                    context_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    context_serialized = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_simulate_citizen_pass_work_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_simulate_citizen_pass_work_orders_simulate_citizen_pass_tas~",
                        column: x => x.cortex_task_id,
                        principalTable: "simulate_citizen_pass_tasks",
                        principalColumn: "cortex_task_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_simulate_citizen_pass_work_orders_status",
                table: "simulate_citizen_pass_work_orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ux_simulate_citizen_pass_work_orders_cortex_task",
                table: "simulate_citizen_pass_work_orders",
                column: "cortex_task_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_simulate_citizen_pass_work_orders_external_request",
                table: "simulate_citizen_pass_work_orders",
                column: "external_request_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "simulate_citizen_pass_work_orders");
        }
    }
}
