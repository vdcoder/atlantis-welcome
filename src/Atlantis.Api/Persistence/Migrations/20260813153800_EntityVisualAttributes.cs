using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class EntityVisualAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_entities",
                table: "entities");

            migrationBuilder.DropIndex(
                name: "ux_entities_world_id_entity_id",
                table: "entities");

            migrationBuilder.DropColumn(
                name: "id",
                table: "entities");

            migrationBuilder.RenameColumn(
                name: "entity_id",
                table: "entities",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_entities",
                table: "entities",
                column: "id");

            migrationBuilder.CreateTable(
                name: "visual_attribute_definitions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visual_attribute_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "visual_attribute_values",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    attribute_definition_id = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_text = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visual_attribute_values", x => x.id);
                    table.ForeignKey(
                        name: "FK_visual_attribute_values_visual_attribute_definitions_attrib~",
                        column: x => x.attribute_definition_id,
                        principalTable: "visual_attribute_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "entity_visual_attributes",
                columns: table => new
                {
                    entity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    attribute_value_id = table.Column<int>(type: "integer", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_visual_attributes", x => new { x.entity_id, x.attribute_value_id });
                    table.CheckConstraint("ck_entity_visual_attributes_sequence", "\"sequence\" >= 1 AND \"sequence\" <= 5");
                    table.ForeignKey(
                        name: "FK_entity_visual_attributes_entities_entity_id",
                        column: x => x.entity_id,
                        principalTable: "entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_entity_visual_attributes_visual_attribute_values_attribute_~",
                        column: x => x.attribute_value_id,
                        principalTable: "visual_attribute_values",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_entities_world_id",
                table: "entities",
                column: "world_id");

            migrationBuilder.CreateIndex(
                name: "IX_entity_visual_attributes_attribute_value_id",
                table: "entity_visual_attributes",
                column: "attribute_value_id");

            migrationBuilder.CreateIndex(
                name: "IX_entity_visual_attributes_entity_id_sequence",
                table: "entity_visual_attributes",
                columns: new[] { "entity_id", "sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_visual_attribute_definitions_name",
                table: "visual_attribute_definitions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_visual_attribute_values_attribute_definition_id_value",
                table: "visual_attribute_values",
                columns: new[] { "attribute_definition_id", "value" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entity_visual_attributes");

            migrationBuilder.DropTable(
                name: "visual_attribute_values");

            migrationBuilder.DropTable(
                name: "visual_attribute_definitions");

            migrationBuilder.DropIndex(
                name: "IX_entities_world_id",
                table: "entities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_entities",
                table: "entities");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "entities",
                newName: "entity_id");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "entities",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_entities",
                table: "entities",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ux_entities_world_id_entity_id",
                table: "entities",
                columns: new[] { "world_id", "entity_id" },
                unique: true);
        }
    }
}
