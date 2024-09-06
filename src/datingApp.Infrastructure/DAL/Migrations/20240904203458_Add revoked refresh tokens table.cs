using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addrevokedrefreshtokenstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeletedEntityDto",
                table: "DeletedEntityDto");

            migrationBuilder.RenameTable(
                name: "DeletedEntityDto",
                newName: "DeletedEntities");

            migrationBuilder.RenameIndex(
                name: "IX_DeletedEntityDto_Id",
                table: "DeletedEntities",
                newName: "IX_DeletedEntities_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeletedEntities",
                table: "DeletedEntities",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RevokedRefreshTokens",
                columns: table => new
                {
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevokedRefreshTokens", x => x.Token);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RevokedRefreshTokens_Token",
                table: "RevokedRefreshTokens",
                column: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RevokedRefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeletedEntities",
                table: "DeletedEntities");

            migrationBuilder.RenameTable(
                name: "DeletedEntities",
                newName: "DeletedEntityDto");

            migrationBuilder.RenameIndex(
                name: "IX_DeletedEntities_Id",
                table: "DeletedEntityDto",
                newName: "IX_DeletedEntityDto_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeletedEntityDto",
                table: "DeletedEntityDto",
                column: "Id");
        }
    }
}
