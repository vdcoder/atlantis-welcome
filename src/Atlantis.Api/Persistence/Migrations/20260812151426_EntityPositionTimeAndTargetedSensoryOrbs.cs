using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class EntityPositionTimeAndTargetedSensoryOrbs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "private_message_delivered_at",
                table: "entities");

            migrationBuilder.DropColumn(
                name: "private_message_sender_id",
                table: "entities");

            migrationBuilder.DropColumn(
                name: "private_message_sequence",
                table: "entities");

            migrationBuilder.DropColumn(
                name: "private_message_text",
                table: "entities");

            migrationBuilder.AddColumn<DateTime>(
                name: "position_changed_at",
                table: "entities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE entities AS e
                SET position_changed_at = w.time
                FROM worlds AS w
                WHERE e.world_id = w.id;
                """);

            migrationBuilder.AlterColumn<DateTime>(
                name: "position_changed_at",
                table: "entities",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "sensory_orb_targets",
                columns: table => new
                {
                    orb_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_entity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensory_orb_targets", x => new { x.orb_id, x.target_entity_id });
                    table.ForeignKey(
                        name: "FK_sensory_orb_targets_sensory_orbs_orb_id",
                        column: x => x.orb_id,
                        principalTable: "sensory_orbs",
                        principalColumn: "orb_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sensory_orb_targets");

            migrationBuilder.DropColumn(
                name: "position_changed_at",
                table: "entities");

            migrationBuilder.AddColumn<DateTime>(
                name: "private_message_delivered_at",
                table: "entities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "private_message_sender_id",
                table: "entities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "private_message_sequence",
                table: "entities",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "private_message_text",
                table: "entities",
                type: "text",
                nullable: true);
        }
    }
}
