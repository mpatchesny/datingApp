using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addindexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Photos_UserId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MatchId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Matches_UserId1",
                table: "Matches");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_Lat_Lon",
                table: "UserSettings",
                columns: new[] { "Lat", "Lon" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Sex_DateOfBirth",
                table: "Users",
                columns: new[] { "Sex", "DateOfBirth" });

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwipedById_SwipedWhoId_Like",
                table: "Swipes",
                columns: new[] { "SwipedById", "SwipedWhoId", "Like" });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId_Oridinal",
                table: "Photos",
                columns: new[] { "UserId", "Oridinal" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MatchId_CreatedAt",
                table: "Messages",
                columns: new[] { "MatchId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_UserId1_UserId2_CreatedAt",
                table: "Matches",
                columns: new[] { "UserId1", "UserId2", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSettings_Lat_Lon",
                table: "UserSettings");

            migrationBuilder.DropIndex(
                name: "IX_Users_Sex_DateOfBirth",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Swipes_SwipedById_SwipedWhoId_Like",
                table: "Swipes");

            migrationBuilder.DropIndex(
                name: "IX_Photos_UserId_Oridinal",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MatchId_CreatedAt",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Matches_UserId1_UserId2_CreatedAt",
                table: "Matches");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId",
                table: "Photos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MatchId",
                table: "Messages",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_UserId1",
                table: "Matches",
                column: "UserId1");
        }
    }
}
