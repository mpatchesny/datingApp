using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MatchdetailaddrelationwithUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MatchDetail_UserId",
                table: "MatchDetail",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchDetail_Users_UserId",
                table: "MatchDetail",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchDetail_Users_UserId",
                table: "MatchDetail");

            migrationBuilder.DropIndex(
                name: "IX_MatchDetail_UserId",
                table: "MatchDetail");
        }
    }
}
