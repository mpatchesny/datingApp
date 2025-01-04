using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addindicestomatchremoveindexfromswipes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Swipes_SwipedById_SwipedWhoId_Like",
                table: "Swipes");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CreatedAt",
                table: "Matches",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_LastActivityTime",
                table: "Matches",
                column: "LastActivityTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Matches_CreatedAt",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_LastActivityTime",
                table: "Matches");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwipedById_SwipedWhoId_Like",
                table: "Swipes",
                columns: new[] { "SwipedById", "SwipedWhoId", "Like" },
                unique: true);
        }
    }
}
