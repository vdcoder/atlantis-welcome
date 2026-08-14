using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddingAgentLoopPhase3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PrivateMessageDeliveredAt",
                table: "Entities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrivateMessageSenderId",
                table: "Entities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PrivateMessageSequence",
                table: "Entities",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrivateMessageText",
                table: "Entities",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrivateMessageDeliveredAt",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "PrivateMessageSenderId",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "PrivateMessageSequence",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "PrivateMessageText",
                table: "Entities");
        }
    }
}
