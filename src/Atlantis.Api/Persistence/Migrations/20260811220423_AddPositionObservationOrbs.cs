using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionObservationOrbs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "position_observation_orbs",
                columns: table => new
                {
                    orb_id = table.Column<Guid>(type: "uuid", nullable: false),
                    observed_entity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_position_observation_orbs", x => x.orb_id);
                    table.ForeignKey(
                        name: "FK_position_observation_orbs_orbs_orb_id",
                        column: x => x.orb_id,
                        principalTable: "orbs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "position_observation_orbs");
        }
    }
}
