using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mis.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamMatchStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "team_match_statistics",
                schema: "football",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FixtureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PossessionPercent = table.Column<int>(type: "int", nullable: true),
                    Shots = table.Column<int>(type: "int", nullable: true),
                    ShotsOnTarget = table.Column<int>(type: "int", nullable: true),
                    Corners = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_match_statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_team_match_statistics_fixtures_FixtureId",
                        column: x => x.FixtureId,
                        principalSchema: "football",
                        principalTable: "fixtures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_team_match_statistics_teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "football",
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_team_match_statistics_FixtureId_TeamId",
                schema: "football",
                table: "team_match_statistics",
                columns: new[] { "FixtureId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_team_match_statistics_TeamId",
                schema: "football",
                table: "team_match_statistics",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "team_match_statistics",
                schema: "football");
        }
    }
}
