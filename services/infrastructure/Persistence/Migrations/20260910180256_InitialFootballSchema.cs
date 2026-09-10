using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mis.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialFootballSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "football");

            migrationBuilder.CreateTable(
                name: "competitions",
                schema: "football",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_competitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                schema: "football",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seasons",
                schema: "football",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seasons_competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalSchema: "football",
                        principalTable: "competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fixtures",
                schema: "football",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HomeTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AwayTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KickoffUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RegulationHomeGoals = table.Column<int>(type: "int", nullable: true),
                    RegulationAwayGoals = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fixtures", x => x.Id);
                    table.CheckConstraint("CK_fixtures_distinct_teams", "[HomeTeamId] <> [AwayTeamId]");
                    table.ForeignKey(
                        name: "FK_fixtures_seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalSchema: "football",
                        principalTable: "seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fixtures_teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalSchema: "football",
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fixtures_teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalSchema: "football",
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fixtures_AwayTeamId",
                schema: "football",
                table: "fixtures",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_fixtures_HomeTeamId",
                schema: "football",
                table: "fixtures",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_fixtures_KickoffUtc_Id",
                schema: "football",
                table: "fixtures",
                columns: new[] { "KickoffUtc", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_fixtures_SeasonId_KickoffUtc_Id",
                schema: "football",
                table: "fixtures",
                columns: new[] { "SeasonId", "KickoffUtc", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_seasons_CompetitionId_Label",
                schema: "football",
                table: "seasons",
                columns: new[] { "CompetitionId", "Label" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fixtures",
                schema: "football");

            migrationBuilder.DropTable(
                name: "seasons",
                schema: "football");

            migrationBuilder.DropTable(
                name: "teams",
                schema: "football");

            migrationBuilder.DropTable(
                name: "competitions",
                schema: "football");
        }
    }
}
