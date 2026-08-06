using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class UseScheduledCortexJobDeactivation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
        """
        UPDATE cortex_job_definitions
        SET deactivated_at =
            COALESCE(deactivated_at, CURRENT_TIMESTAMP)
        WHERE is_active = false;
        """);

            migrationBuilder.DropIndex(
                name: "ix_cortex_job_definitions_is_active",
                table: "cortex_job_definitions");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "cortex_job_definitions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
        name: "is_active",
        table: "cortex_job_definitions",
        type: "boolean",
        nullable: false,
        defaultValue: true);

            migrationBuilder.Sql(
                """
        UPDATE cortex_job_definitions
        SET is_active =
            deactivated_at IS NULL
            OR deactivated_at > CURRENT_TIMESTAMP;
        """);

            migrationBuilder.CreateIndex(
                name: "ix_cortex_job_definitions_is_active",
                table: "cortex_job_definitions",
                column: "is_active");
        }
    }
}
