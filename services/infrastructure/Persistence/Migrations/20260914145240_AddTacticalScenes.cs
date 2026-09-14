using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mis.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTacticalScenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tactical_scenes",
                schema: "football",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FixtureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    StateJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tactical_scenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tactical_scenes_fixtures_FixtureId",
                        column: x => x.FixtureId,
                        principalSchema: "football",
                        principalTable: "fixtures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tactical_scenes_FixtureId_CreatedAt",
                schema: "football",
                table: "tactical_scenes",
                columns: new[] { "FixtureId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tactical_scenes",
                schema: "football");
        }
    }
}
