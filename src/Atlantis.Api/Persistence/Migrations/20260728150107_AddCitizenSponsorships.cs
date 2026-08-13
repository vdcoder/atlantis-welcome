using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCitizenSponsorships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "citizen_sponsorships",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    citizen_entity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sponsor_entity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    funding_account_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    activated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    end_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citizen_sponsorships", x => x.id);
                    table.CheckConstraint("ck_citizen_sponsorships_distinct_entities", "\"citizen_entity_id\" <> \"sponsor_entity_id\"");
                    table.CheckConstraint("ck_citizen_sponsorships_end_reason", "\"end_reason\" IS NULL OR length(btrim(\"end_reason\")) > 0");
                    table.CheckConstraint("ck_citizen_sponsorships_status", "\"status\" IN (1, 2, 3, 4)");
                    table.CheckConstraint("ck_citizen_sponsorships_status_dates", "                (\r\n                    \"status\" = 1\r\n                    AND \"activated_at\" IS NULL\r\n                    AND \"ended_at\" IS NULL\r\n                )\r\n                OR\r\n                (\r\n                    \"status\" = 2\r\n                    AND \"activated_at\" IS NOT NULL\r\n                    AND \"activated_at\" >= \"created_at\"\r\n                    AND \"ended_at\" IS NULL\r\n                )\r\n                OR\r\n                (\r\n                    \"status\" IN (3, 4)\r\n                    AND \"activated_at\" IS NOT NULL\r\n                    AND \"activated_at\" >= \"created_at\"\r\n                    AND \"ended_at\" IS NOT NULL\r\n                    AND \"ended_at\" >= \"activated_at\"\r\n                )");
                    table.ForeignKey(
                        name: "FK_citizen_sponsorships_money_accounts_funding_account_id",
                        column: x => x.funding_account_id,
                        principalTable: "money_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cortex_job_inbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inbox_id = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    cortex_task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cortex_task_assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    payload_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    payload_serialized = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cortex_job_inbox_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_cortex_job_inbox_messages_cortex_task_assignments_cortex_ta~",
                        column: x => x.cortex_task_assignment_id,
                        principalTable: "cortex_task_assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cortex_job_inbox_messages_cortex_tasks_cortex_task_id",
                        column: x => x.cortex_task_id,
                        principalTable: "cortex_tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cortex_task_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cortex_task_assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    result_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    result_serialized = table.Column<string>(type: "jsonb", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cortex_task_results", x => x.id);
                    table.ForeignKey(
                        name: "FK_cortex_task_results_cortex_task_assignments_cortex_task_ass~",
                        column: x => x.cortex_task_assignment_id,
                        principalTable: "cortex_task_assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_citizen_sponsorships_funding_account",
                table: "citizen_sponsorships",
                column: "funding_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_citizen_sponsorships_sponsor",
                table: "citizen_sponsorships",
                column: "sponsor_entity_id");

            migrationBuilder.CreateIndex(
                name: "ux_citizen_sponsorships_active_citizen",
                table: "citizen_sponsorships",
                column: "citizen_entity_id",
                unique: true,
                filter: "\"status\" = 2");

            migrationBuilder.CreateIndex(
                name: "IX_cortex_job_inbox_messages_cortex_task_id",
                table: "cortex_job_inbox_messages",
                column: "cortex_task_id");

            migrationBuilder.CreateIndex(
                name: "ix_cortex_job_inbox_messages_delivery",
                table: "cortex_job_inbox_messages",
                columns: new[] { "inbox_id", "processed_at", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ux_cortex_job_inbox_messages_assignment_type",
                table: "cortex_job_inbox_messages",
                columns: new[] { "cortex_task_assignment_id", "type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_cortex_task_results_assignment",
                table: "cortex_task_results",
                column: "cortex_task_assignment_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "citizen_sponsorships");

            migrationBuilder.DropTable(
                name: "cortex_job_inbox_messages");

            migrationBuilder.DropTable(
                name: "cortex_task_results");
        }
    }
}
