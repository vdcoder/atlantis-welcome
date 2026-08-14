using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class EntityVoiceAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "voice_attribute_definitions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voice_attribute_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "voice_attribute_values",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    attribute_definition_id = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_text = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voice_attribute_values", x => x.id);
                    table.ForeignKey(
                        name: "FK_voice_attribute_values_voice_attribute_definitions_attribut~",
                        column: x => x.attribute_definition_id,
                        principalTable: "voice_attribute_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "entity_voice_attributes",
                columns: table => new
                {
                    entity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    attribute_value_id = table.Column<int>(type: "integer", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_voice_attributes", x => new { x.entity_id, x.attribute_value_id });
                    table.CheckConstraint("ck_entity_voice_attributes_sequence", "\"sequence\" >= 1 AND \"sequence\" <= 5");
                    table.ForeignKey(
                        name: "FK_entity_voice_attributes_entities_entity_id",
                        column: x => x.entity_id,
                        principalTable: "entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_entity_voice_attributes_voice_attribute_values_attribute_va~",
                        column: x => x.attribute_value_id,
                        principalTable: "voice_attribute_values",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_entity_voice_attributes_attribute_value_id",
                table: "entity_voice_attributes",
                column: "attribute_value_id");

            migrationBuilder.CreateIndex(
                name: "IX_entity_voice_attributes_entity_id_sequence",
                table: "entity_voice_attributes",
                columns: new[] { "entity_id", "sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_voice_attribute_definitions_name",
                table: "voice_attribute_definitions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_voice_attribute_values_attribute_definition_id_value",
                table: "voice_attribute_values",
                columns: new[] { "attribute_definition_id", "value" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entity_voice_attributes");

            migrationBuilder.DropTable(
                name: "voice_attribute_values");

            migrationBuilder.DropTable(
                name: "voice_attribute_definitions");
        }
    }
}
