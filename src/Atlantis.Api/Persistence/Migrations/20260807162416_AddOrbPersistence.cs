using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrbPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orbs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    world_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    source_entity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    attachment_entity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    attachment_position_x = table.Column<float>(type: "real", nullable: false),
                    attachment_position_y = table.Column<float>(type: "real", nullable: false),
                    attachment_position_z = table.Column<float>(type: "real", nullable: false),
                    radius = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orbs", x => x.id);
                    table.CheckConstraint("ck_orbs_expiration", "expires_at IS NULL OR expires_at > created_at");
                    table.CheckConstraint("ck_orbs_radius", "radius >= 0");
                    table.ForeignKey(
                        name: "FK_orbs_worlds_world_id",
                        column: x => x.world_id,
                        principalTable: "worlds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sensory_orbs",
                columns: table => new
                {
                    orb_id = table.Column<Guid>(type: "uuid", nullable: false),
                    modality = table.Column<int>(type: "integer", nullable: false),
                    intensity = table.Column<float>(type: "real", nullable: false),
                    payload = table.Column<JsonDocument>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensory_orbs", x => x.orb_id);
                    table.CheckConstraint("ck_sensory_orbs_intensity", "intensity >= 0 AND intensity <= 1");
                    table.ForeignKey(
                        name: "FK_sensory_orbs_orbs_orb_id",
                        column: x => x.orb_id,
                        principalTable: "orbs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_orbs_world_id",
                table: "orbs",
                column: "world_id");

            migrationBuilder.CreateIndex(
                name: "ix_orbs_world_id_expires_at",
                table: "orbs",
                columns: new[] { "world_id", "expires_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sensory_orbs");

            migrationBuilder.DropTable(
                name: "orbs");
        }
    }
}
