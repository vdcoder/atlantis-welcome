using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDevelopmentPredictionContextContractVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_development_recorded_predictions_exact_input",
                table: "development_recorded_predictions");

            migrationBuilder.DropIndex(
                name: "ux_development_prediction_requests_exact_input",
                table: "development_prediction_requests");

            migrationBuilder.AddColumn<int>(
                name: "context_contract_version",
                table: "development_recorded_predictions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "context_contract_version",
                table: "development_prediction_requests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ux_development_recorded_predictions_exact_input",
                table: "development_recorded_predictions",
                columns: new[] { "development_world_id", "scenario_id", "simulated_citizen_id", "sequence_number", "context_contract_version", "context_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_development_prediction_requests_exact_input",
                table: "development_prediction_requests",
                columns: new[] { "development_world_id", "scenario_id", "simulated_citizen_id", "sequence_number", "context_contract_version", "context_hash" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_development_recorded_predictions_exact_input",
                table: "development_recorded_predictions");

            migrationBuilder.DropIndex(
                name: "ux_development_prediction_requests_exact_input",
                table: "development_prediction_requests");

            migrationBuilder.DropColumn(
                name: "context_contract_version",
                table: "development_recorded_predictions");

            migrationBuilder.DropColumn(
                name: "context_contract_version",
                table: "development_prediction_requests");

            migrationBuilder.CreateIndex(
                name: "ux_development_recorded_predictions_exact_input",
                table: "development_recorded_predictions",
                columns: new[] { "development_world_id", "scenario_id", "simulated_citizen_id", "sequence_number", "context_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_development_prediction_requests_exact_input",
                table: "development_prediction_requests",
                columns: new[] { "development_world_id", "scenario_id", "simulated_citizen_id", "sequence_number", "context_hash" },
                unique: true);
        }
    }
}
