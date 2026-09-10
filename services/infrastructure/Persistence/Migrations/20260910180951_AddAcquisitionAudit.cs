using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mis.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcquisitionAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "acquisition");

            migrationBuilder.CreateTable(
                name: "provider_entity_mappings",
                schema: "acquisition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CanonicalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_provider_entity_mappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sync_runs",
                schema: "acquisition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequestedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ImportedFixtureCount = table.Column<int>(type: "int", nullable: false),
                    ErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sync_runs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_provider_entity_mappings_Provider_ResourceType_ExternalId",
                schema: "acquisition",
                table: "provider_entity_mappings",
                columns: new[] { "Provider", "ResourceType", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sync_runs_RequestedAt",
                schema: "acquisition",
                table: "sync_runs",
                column: "RequestedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "provider_entity_mappings",
                schema: "acquisition");

            migrationBuilder.DropTable(
                name: "sync_runs",
                schema: "acquisition");
        }
    }
}
