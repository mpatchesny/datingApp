using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddreferencetoUserstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwippedById",
                table: "Swipes",
                column: "SwippedById");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwippedWhoId",
                table: "Swipes",
                column: "SwippedWhoId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SendFromId",
                table: "Messages",
                column: "SendFromId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SendToId",
                table: "Messages",
                column: "SendToId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_UserId1",
                table: "Matches",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_UserId2",
                table: "Matches",
                column: "UserId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Users_UserId1",
                table: "Matches",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Users_UserId2",
                table: "Matches",
                column: "UserId2",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SendFromId",
                table: "Messages",
                column: "SendFromId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SendToId",
                table: "Messages",
                column: "SendToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Swipes_Users_SwippedById",
                table: "Swipes",
                column: "SwippedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Swipes_Users_SwippedWhoId",
                table: "Swipes",
                column: "SwippedWhoId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Users_UserId1",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Users_UserId2",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SendFromId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SendToId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Swipes_Users_SwippedById",
                table: "Swipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Swipes_Users_SwippedWhoId",
                table: "Swipes");

            migrationBuilder.DropIndex(
                name: "IX_Swipes_SwippedById",
                table: "Swipes");

            migrationBuilder.DropIndex(
                name: "IX_Swipes_SwippedWhoId",
                table: "Swipes");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SendFromId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SendToId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Matches_UserId1",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_UserId2",
                table: "Matches");
        }
    }
}
