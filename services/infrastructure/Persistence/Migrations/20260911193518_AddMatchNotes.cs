using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mis.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "match_notes",
                schema: "football",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FixtureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Minute = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CapturedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_match_notes_fixtures_FixtureId",
                        column: x => x.FixtureId,
                        principalSchema: "football",
                        principalTable: "fixtures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_notes_FixtureId_Minute_Id",
                schema: "football",
                table: "match_notes",
                columns: new[] { "FixtureId", "Minute", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_notes",
                schema: "football");
        }
    }
}
