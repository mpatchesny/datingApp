using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addaccescode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessCodeDto",
                columns: table => new
                {
                    EmailOrPhone = table.Column<string>(type: "text", nullable: false),
                    AccessCode = table.Column<string>(type: "text", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Expiry = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessCodeDto", x => x.EmailOrPhone);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessCodeDto_EmailOrPhone",
                table: "AccessCodeDto",
                column: "EmailOrPhone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessCodeDto");
        }
    }
}
