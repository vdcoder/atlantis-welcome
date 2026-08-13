using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class DroppingEntityUtterance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "utterance_sequence",
                table: "entities");

            migrationBuilder.DropColumn(
                name: "utterance_spoken_at",
                table: "entities");

            migrationBuilder.DropColumn(
                name: "utterance_text",
                table: "entities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "utterance_sequence",
                table: "entities",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "utterance_spoken_at",
                table: "entities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "utterance_text",
                table: "entities",
                type: "text",
                nullable: true);
        }
    }
}
