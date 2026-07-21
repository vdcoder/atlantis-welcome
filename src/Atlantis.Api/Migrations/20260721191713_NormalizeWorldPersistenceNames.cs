using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeWorldPersistenceNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Worlds_WorldId",
                table: "Entities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Worlds",
                table: "Worlds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Entities",
                table: "Entities");

            migrationBuilder.RenameTable(
                name: "Worlds",
                newName: "worlds");

            migrationBuilder.RenameTable(
                name: "Entities",
                newName: "entities");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "worlds",
                newName: "time");

            migrationBuilder.RenameColumn(
                name: "Revision",
                table: "worlds",
                newName: "revision");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "worlds",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WorldId",
                table: "worlds",
                newName: "world_id");

            migrationBuilder.RenameIndex(
                name: "IX_Worlds_WorldId",
                table: "worlds",
                newName: "ux_worlds_world_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "entities",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "entities",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "entities",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WorldId",
                table: "entities",
                newName: "world_id");

            migrationBuilder.RenameColumn(
                name: "UtteranceText",
                table: "entities",
                newName: "utterance_text");

            migrationBuilder.RenameColumn(
                name: "UtteranceSpokenAt",
                table: "entities",
                newName: "utterance_spoken_at");

            migrationBuilder.RenameColumn(
                name: "UtteranceSequence",
                table: "entities",
                newName: "utterance_sequence");

            migrationBuilder.RenameColumn(
                name: "PrivateMessageText",
                table: "entities",
                newName: "private_message_text");

            migrationBuilder.RenameColumn(
                name: "PrivateMessageSequence",
                table: "entities",
                newName: "private_message_sequence");

            migrationBuilder.RenameColumn(
                name: "PrivateMessageSenderId",
                table: "entities",
                newName: "private_message_sender_id");

            migrationBuilder.RenameColumn(
                name: "PrivateMessageDeliveredAt",
                table: "entities",
                newName: "private_message_delivered_at");

            migrationBuilder.RenameColumn(
                name: "PositionZ",
                table: "entities",
                newName: "position_z");

            migrationBuilder.RenameColumn(
                name: "PositionY",
                table: "entities",
                newName: "position_y");

            migrationBuilder.RenameColumn(
                name: "PositionX",
                table: "entities",
                newName: "position_x");

            migrationBuilder.RenameColumn(
                name: "PlaceId",
                table: "entities",
                newName: "place_id");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "entities",
                newName: "entity_id");

            migrationBuilder.RenameIndex(
                name: "IX_Entities_WorldId_EntityId",
                table: "entities",
                newName: "ux_entities_world_id_entity_id");

            migrationBuilder.AlterColumn<string>(
                name: "private_message_sender_id",
                table: "entities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_worlds",
                table: "worlds",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_entities",
                table: "entities",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_entities_worlds_world_id",
                table: "entities",
                column: "world_id",
                principalTable: "worlds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_entities_worlds_world_id",
                table: "entities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_worlds",
                table: "worlds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_entities",
                table: "entities");

            migrationBuilder.RenameTable(
                name: "worlds",
                newName: "Worlds");

            migrationBuilder.RenameTable(
                name: "entities",
                newName: "Entities");

            migrationBuilder.RenameColumn(
                name: "time",
                table: "Worlds",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "revision",
                table: "Worlds",
                newName: "Revision");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Worlds",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "world_id",
                table: "Worlds",
                newName: "WorldId");

            migrationBuilder.RenameIndex(
                name: "ux_worlds_world_id",
                table: "Worlds",
                newName: "IX_Worlds_WorldId");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Entities",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Entities",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Entities",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "world_id",
                table: "Entities",
                newName: "WorldId");

            migrationBuilder.RenameColumn(
                name: "utterance_text",
                table: "Entities",
                newName: "UtteranceText");

            migrationBuilder.RenameColumn(
                name: "utterance_spoken_at",
                table: "Entities",
                newName: "UtteranceSpokenAt");

            migrationBuilder.RenameColumn(
                name: "utterance_sequence",
                table: "Entities",
                newName: "UtteranceSequence");

            migrationBuilder.RenameColumn(
                name: "private_message_text",
                table: "Entities",
                newName: "PrivateMessageText");

            migrationBuilder.RenameColumn(
                name: "private_message_sequence",
                table: "Entities",
                newName: "PrivateMessageSequence");

            migrationBuilder.RenameColumn(
                name: "private_message_sender_id",
                table: "Entities",
                newName: "PrivateMessageSenderId");

            migrationBuilder.RenameColumn(
                name: "private_message_delivered_at",
                table: "Entities",
                newName: "PrivateMessageDeliveredAt");

            migrationBuilder.RenameColumn(
                name: "position_z",
                table: "Entities",
                newName: "PositionZ");

            migrationBuilder.RenameColumn(
                name: "position_y",
                table: "Entities",
                newName: "PositionY");

            migrationBuilder.RenameColumn(
                name: "position_x",
                table: "Entities",
                newName: "PositionX");

            migrationBuilder.RenameColumn(
                name: "place_id",
                table: "Entities",
                newName: "PlaceId");

            migrationBuilder.RenameColumn(
                name: "entity_id",
                table: "Entities",
                newName: "EntityId");

            migrationBuilder.RenameIndex(
                name: "ux_entities_world_id_entity_id",
                table: "Entities",
                newName: "IX_Entities_WorldId_EntityId");

            migrationBuilder.AlterColumn<string>(
                name: "PrivateMessageSenderId",
                table: "Entities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Worlds",
                table: "Worlds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Entities",
                table: "Entities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_Worlds_WorldId",
                table: "Entities",
                column: "WorldId",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
