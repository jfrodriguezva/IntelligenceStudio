using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mis.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "players",
                schema: "football",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "match_events",
                schema: "football",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FixtureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Minute = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_match_events_fixtures_FixtureId",
                        column: x => x.FixtureId,
                        principalSchema: "football",
                        principalTable: "fixtures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_match_events_players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "football",
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "squad_memberships",
                schema: "football",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_squad_memberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_squad_memberships_players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "football",
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_squad_memberships_teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "football",
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_events_FixtureId_Minute_Id",
                schema: "football",
                table: "match_events",
                columns: new[] { "FixtureId", "Minute", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_match_events_PlayerId",
                schema: "football",
                table: "match_events",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_squad_memberships_PlayerId_TeamId_ValidFrom",
                schema: "football",
                table: "squad_memberships",
                columns: new[] { "PlayerId", "TeamId", "ValidFrom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_squad_memberships_TeamId",
                schema: "football",
                table: "squad_memberships",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_events",
                schema: "football");

            migrationBuilder.DropTable(
                name: "squad_memberships",
                schema: "football");

            migrationBuilder.DropTable(
                name: "players",
                schema: "football");
        }
    }
}
