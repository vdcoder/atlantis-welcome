using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddingAgentLoopPhase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UtteranceSequence",
                table: "Entities",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UtteranceSpokenAt",
                table: "Entities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UtteranceText",
                table: "Entities",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UtteranceSequence",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "UtteranceSpokenAt",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "UtteranceText",
                table: "Entities");
        }
    }
}
