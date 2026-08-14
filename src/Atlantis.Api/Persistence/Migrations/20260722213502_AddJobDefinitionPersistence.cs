using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddJobDefinitionPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "job_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employer_account_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    qualifications = table.Column<string>(type: "text", nullable: false),
                    completion_inbox_id = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    failure_inbox_id = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deactivated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job_schedule_conditions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    condition_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    configuration_json = table.Column<string>(type: "jsonb", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_schedule_conditions", x => x.id);
                    table.ForeignKey(
                        name: "FK_job_schedule_conditions_job_definitions_job_definition_id",
                        column: x => x.job_definition_id,
                        principalTable: "job_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_job_definitions_is_active",
                table: "job_definitions",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ux_job_definitions_employer_name",
                table: "job_definitions",
                columns: new[] { "employer_account_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_job_schedule_conditions_type",
                table: "job_schedule_conditions",
                column: "condition_type");

            migrationBuilder.CreateIndex(
                name: "ux_job_schedule_conditions_definition_order",
                table: "job_schedule_conditions",
                columns: new[] { "job_definition_id", "sort_order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_schedule_conditions");

            migrationBuilder.DropTable(
                name: "job_definitions");
        }
    }
}
