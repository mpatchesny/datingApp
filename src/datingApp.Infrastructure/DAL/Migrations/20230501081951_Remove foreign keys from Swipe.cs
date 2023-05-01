using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveforeignkeysfromSwipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Swipes_Users_SwipedById",
                table: "Swipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Swipes_Users_SwipedWhoId",
                table: "Swipes");

            migrationBuilder.DropIndex(
                name: "IX_Swipes_SwipedWhoId",
                table: "Swipes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwipedWhoId",
                table: "Swipes",
                column: "SwipedWhoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Swipes_Users_SwipedById",
                table: "Swipes",
                column: "SwipedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Swipes_Users_SwipedWhoId",
                table: "Swipes",
                column: "SwipedWhoId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
