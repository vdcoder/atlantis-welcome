using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class WorkingMemoryV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "citizen_memory_stream_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    citizen_id = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    content_token_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citizen_memory_stream_entries", x => x.id);
                    table.CheckConstraint("ck_citizen_memory_stream_entry_token_count", "content_token_count > 0");
                });

            migrationBuilder.CreateTable(
                name: "citizen_working_memory_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    citizen_id = table.Column<string>(type: "text", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    content_token_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citizen_working_memory_lines", x => x.id);
                    table.CheckConstraint("ck_citizen_working_memory_line_number", "line_number >= 1 AND line_number <= 40");
                    table.CheckConstraint("ck_citizen_working_memory_line_token_count", "content_token_count >= 0 AND content_token_count <= 128");
                });

            migrationBuilder.CreateIndex(
                name: "IX_citizen_memory_stream_entries_citizen_id_created_at_id",
                table: "citizen_memory_stream_entries",
                columns: new[] { "citizen_id", "created_at", "id" },
                descending: new[] { false, true, true });

            migrationBuilder.CreateIndex(
                name: "IX_citizen_working_memory_lines_citizen_id_line_number_created~",
                table: "citizen_working_memory_lines",
                columns: new[] { "citizen_id", "line_number", "created_at", "id" },
                descending: new[] { false, false, true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "citizen_memory_stream_entries");

            migrationBuilder.DropTable(
                name: "citizen_working_memory_lines");
        }
    }
}
