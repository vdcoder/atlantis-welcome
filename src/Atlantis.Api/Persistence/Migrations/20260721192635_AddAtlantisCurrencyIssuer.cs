using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAtlantisCurrencyIssuer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_ledger_entries_transaction_id",
                table: "ledger_entries");

            migrationBuilder.CreateIndex(
                name: "ux_ledger_entries_transaction_id_account_id",
                table: "ledger_entries",
                columns: new[] { "transaction_id", "account_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_ledger_entries_transaction_id_account_id",
                table: "ledger_entries");

            migrationBuilder.CreateIndex(
                name: "ix_ledger_entries_transaction_id",
                table: "ledger_entries",
                column: "transaction_id");
        }
    }
}
