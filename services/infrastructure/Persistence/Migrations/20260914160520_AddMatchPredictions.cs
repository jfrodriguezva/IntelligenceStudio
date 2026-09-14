using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mis.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchPredictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "match_predictions",
                schema: "football",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FixtureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelVersion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataCutoff = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    HomeWin = table.Column<double>(type: "float", nullable: false),
                    Draw = table.Column<double>(type: "float", nullable: false),
                    AwayWin = table.Column<double>(type: "float", nullable: false),
                    HomeStrength = table.Column<double>(type: "float", nullable: false),
                    AwayStrength = table.Column<double>(type: "float", nullable: false),
                    GeneratedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_predictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_match_predictions_fixtures_FixtureId",
                        column: x => x.FixtureId,
                        principalSchema: "football",
                        principalTable: "fixtures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_predictions_FixtureId_GeneratedAt",
                schema: "football",
                table: "match_predictions",
                columns: new[] { "FixtureId", "GeneratedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_predictions",
                schema: "football");
        }
    }
}
