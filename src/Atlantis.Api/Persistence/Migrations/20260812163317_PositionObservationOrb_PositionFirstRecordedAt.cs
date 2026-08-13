using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class PositionObservationOrb_PositionFirstRecordedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "position_first_recorded_at",
                table: "position_observation_orbs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE position_observation_orbs AS p
                SET position_first_recorded_at = o.created_at
                FROM orbs AS o
                WHERE p.orb_id = o.id;
                """);

            migrationBuilder.AlterColumn<DateTime>(
                name: "position_first_recorded_at",
                table: "position_observation_orbs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "position_first_recorded_at",
                table: "position_observation_orbs");
        }
    }
}
