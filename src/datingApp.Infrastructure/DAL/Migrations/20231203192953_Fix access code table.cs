using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Fixaccesscodetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AccessCodeDto",
                table: "AccessCodeDto");

            migrationBuilder.RenameTable(
                name: "AccessCodeDto",
                newName: "AccessCodes");

            migrationBuilder.RenameIndex(
                name: "IX_AccessCodeDto_EmailOrPhone",
                table: "AccessCodes",
                newName: "IX_AccessCodes_EmailOrPhone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccessCodes",
                table: "AccessCodes",
                column: "EmailOrPhone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AccessCodes",
                table: "AccessCodes");

            migrationBuilder.RenameTable(
                name: "AccessCodes",
                newName: "AccessCodeDto");

            migrationBuilder.RenameIndex(
                name: "IX_AccessCodes_EmailOrPhone",
                table: "AccessCodeDto",
                newName: "IX_AccessCodeDto_EmailOrPhone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccessCodeDto",
                table: "AccessCodeDto",
                column: "EmailOrPhone");
        }
    }
}
