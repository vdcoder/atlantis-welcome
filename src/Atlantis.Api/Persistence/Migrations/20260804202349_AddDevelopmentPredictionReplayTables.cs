using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDevelopmentPredictionReplayTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "development_prediction_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    development_world_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    scenario_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    simulated_citizen_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sequence_number = table.Column<long>(type: "bigint", nullable: false),
                    context_serialized = table.Column<string>(type: "text", nullable: false),
                    context_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_development_prediction_requests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "development_recorded_predictions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    development_world_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    scenario_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    simulated_citizen_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sequence_number = table.Column<long>(type: "bigint", nullable: false),
                    context_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    predictions_serialized = table.Column<string>(type: "text", nullable: false),
                    worker_citizen_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_development_recorded_predictions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ux_development_prediction_requests_exact_input",
                table: "development_prediction_requests",
                columns: new[] { "development_world_id", "scenario_id", "simulated_citizen_id", "sequence_number", "context_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_development_recorded_predictions_exact_input",
                table: "development_recorded_predictions",
                columns: new[] { "development_world_id", "scenario_id", "simulated_citizen_id", "sequence_number", "context_hash" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "development_prediction_requests");

            migrationBuilder.DropTable(
                name: "development_recorded_predictions");
        }
    }
}
