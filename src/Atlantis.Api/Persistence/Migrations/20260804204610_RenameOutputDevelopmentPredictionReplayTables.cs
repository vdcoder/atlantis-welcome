using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameOutputDevelopmentPredictionReplayTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "predictions_serialized",
                table: "development_recorded_predictions",
                newName: "cognitive_output_serialized");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cognitive_output_serialized",
                table: "development_recorded_predictions",
                newName: "predictions_serialized");
        }
    }
}
