using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCortexTaskRemunerations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cortex_task_remunerations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cortex_task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cortex_task_assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_citizen_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    employer_account_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    paid_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ledger_transaction_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cortex_task_remunerations", x => x.id);
                    table.ForeignKey(
                        name: "FK_cortex_task_remunerations_cortex_task_assignments_cortex_ta~",
                        column: x => x.cortex_task_assignment_id,
                        principalTable: "cortex_task_assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cortex_task_remunerations_cortex_tasks_cortex_task_id",
                        column: x => x.cortex_task_id,
                        principalTable: "cortex_tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cortex_task_remunerations_pending_order",
                table: "cortex_task_remunerations",
                columns: new[] { "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ux_cortex_task_remunerations_assignment",
                table: "cortex_task_remunerations",
                column: "cortex_task_assignment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_cortex_task_remunerations_ledger_transaction",
                table: "cortex_task_remunerations",
                column: "ledger_transaction_id",
                unique: true,
                filter: "ledger_transaction_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_cortex_task_remunerations_task",
                table: "cortex_task_remunerations",
                column: "cortex_task_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cortex_task_remunerations");
        }
    }
}
