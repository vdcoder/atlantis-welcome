using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCortexJobEmploymentPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cortex_job_applications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cortex_job_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_citizen_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    deposit_account_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    applied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    approved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    rejected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cortex_job_applications", x => x.id);
                    table.ForeignKey(
                        name: "FK_cortex_job_applications_cortex_job_definitions_cortex_job_d~",
                        column: x => x.cortex_job_definition_id,
                        principalTable: "cortex_job_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "worker_cortex_job_qualifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cortex_job_application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_citizen_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    deposit_account_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    cortex_job_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    qualified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    suspended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worker_cortex_job_qualifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_worker_cortex_job_qualifications_cortex_job_applications_co~",
                        column: x => x.cortex_job_application_id,
                        principalTable: "cortex_job_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_worker_cortex_job_qualifications_cortex_job_definitions_cor~",
                        column: x => x.cortex_job_definition_id,
                        principalTable: "cortex_job_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "worker_cortex_job_availability",
                columns: table => new
                {
                    worker_cortex_job_qualification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false),
                    changed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worker_cortex_job_availability", x => x.worker_cortex_job_qualification_id);
                    table.ForeignKey(
                        name: "FK_worker_cortex_job_availability_worker_cortex_job_qualificat~",
                        column: x => x.worker_cortex_job_qualification_id,
                        principalTable: "worker_cortex_job_qualifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cortex_job_applications_cortex_job_definition_id",
                table: "cortex_job_applications",
                column: "cortex_job_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_cortex_job_applications_worker_definition",
                table: "cortex_job_applications",
                columns: new[] { "worker_citizen_id", "cortex_job_definition_id" });

            migrationBuilder.CreateIndex(
                name: "ix_worker_cortex_job_availability_is_available",
                table: "worker_cortex_job_availability",
                column: "is_available");

            migrationBuilder.CreateIndex(
                name: "IX_worker_cortex_job_qualifications_cortex_job_definition_id",
                table: "worker_cortex_job_qualifications",
                column: "cortex_job_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_worker_cortex_job_qualifications_deposit_account",
                table: "worker_cortex_job_qualifications",
                column: "deposit_account_id");

            migrationBuilder.CreateIndex(
                name: "ux_worker_cortex_job_qualifications_application",
                table: "worker_cortex_job_qualifications",
                column: "cortex_job_application_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_worker_cortex_job_qualifications_worker_definition",
                table: "worker_cortex_job_qualifications",
                columns: new[] { "worker_citizen_id", "cortex_job_definition_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "worker_cortex_job_availability");

            migrationBuilder.DropTable(
                name: "worker_cortex_job_qualifications");

            migrationBuilder.DropTable(
                name: "cortex_job_applications");
        }
    }
}
