using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameJobsToCortexJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(
    MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name:
                    "FK_job_schedule_conditions_job_definitions_job_definition_id",
                table:
                    "job_schedule_conditions");

            migrationBuilder.RenameTable(
                name:
                    "job_definitions",
                newName:
                    "cortex_job_definitions");

            migrationBuilder.RenameTable(
                name:
                    "job_schedule_conditions",
                newName:
                    "cortex_job_schedule_conditions");

            migrationBuilder.RenameColumn(
                name:
                    "job_definition_id",
                table:
                    "cortex_job_schedule_conditions",
                newName:
                    "cortex_job_definition_id");

            migrationBuilder.RenameIndex(
                name:
                    "ix_job_definitions_is_active",
                table:
                    "cortex_job_definitions",
                newName:
                    "ix_cortex_job_definitions_is_active");

            migrationBuilder.RenameIndex(
                name:
                    "ux_job_definitions_employer_name",
                table:
                    "cortex_job_definitions",
                newName:
                    "ux_cortex_job_definitions_employer_name");

            migrationBuilder.RenameIndex(
                name:
                    "ix_job_schedule_conditions_type",
                table:
                    "cortex_job_schedule_conditions",
                newName:
                    "ix_cortex_job_schedule_conditions_type");

            migrationBuilder.RenameIndex(
                name:
                    "ux_job_schedule_conditions_definition_order",
                table:
                    "cortex_job_schedule_conditions",
                newName:
                    "ux_cortex_job_schedule_conditions_definition_order");

            // PostgreSQL does not automatically rename these constraints
            // when their tables are renamed.
            migrationBuilder.Sql(
                """
        ALTER TABLE cortex_job_definitions
        RENAME CONSTRAINT "PK_job_definitions"
        TO "PK_cortex_job_definitions";
        """);

            migrationBuilder.Sql(
                """
        ALTER TABLE cortex_job_schedule_conditions
        RENAME CONSTRAINT "PK_job_schedule_conditions"
        TO "PK_cortex_job_schedule_conditions";
        """);

            migrationBuilder.AddForeignKey(
                name:
                    "FK_cortex_job_schedule_conditions_cortex_job_definitions_corte~",
                table:
                    "cortex_job_schedule_conditions",
                column:
                    "cortex_job_definition_id",
                principalTable:
                    "cortex_job_definitions",
                principalColumn:
                    "id",
                onDelete:
                    ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(
    MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name:
                    "FK_cortex_job_schedule_conditions_cortex_job_definitions_corte~",
                table:
                    "cortex_job_schedule_conditions");

            migrationBuilder.Sql(
                """
        ALTER TABLE cortex_job_definitions
        RENAME CONSTRAINT "PK_cortex_job_definitions"
        TO "PK_job_definitions";
        """);

            migrationBuilder.Sql(
                """
        ALTER TABLE cortex_job_schedule_conditions
        RENAME CONSTRAINT "PK_cortex_job_schedule_conditions"
        TO "PK_job_schedule_conditions";
        """);

            migrationBuilder.RenameIndex(
                name:
                    "ix_cortex_job_definitions_is_active",
                table:
                    "cortex_job_definitions",
                newName:
                    "ix_job_definitions_is_active");

            migrationBuilder.RenameIndex(
                name:
                    "ux_cortex_job_definitions_employer_name",
                table:
                    "cortex_job_definitions",
                newName:
                    "ux_job_definitions_employer_name");

            migrationBuilder.RenameIndex(
                name:
                    "ix_cortex_job_schedule_conditions_type",
                table:
                    "cortex_job_schedule_conditions",
                newName:
                    "ix_job_schedule_conditions_type");

            migrationBuilder.RenameIndex(
                name:
                    "ux_cortex_job_schedule_conditions_definition_order",
                table:
                    "cortex_job_schedule_conditions",
                newName:
                    "ux_job_schedule_conditions_definition_order");

            migrationBuilder.RenameColumn(
                name:
                    "cortex_job_definition_id",
                table:
                    "cortex_job_schedule_conditions",
                newName:
                    "job_definition_id");

            migrationBuilder.RenameTable(
                name:
                    "cortex_job_schedule_conditions",
                newName:
                    "job_schedule_conditions");

            migrationBuilder.RenameTable(
                name:
                    "cortex_job_definitions",
                newName:
                    "job_definitions");

            migrationBuilder.AddForeignKey(
                name:
                    "FK_job_schedule_conditions_job_definitions_job_definition_id",
                table:
                    "job_schedule_conditions",
                column:
                    "job_definition_id",
                principalTable:
                    "job_definitions",
                principalColumn:
                    "id",
                onDelete:
                    ReferentialAction.Cascade);
        }
    }
}
