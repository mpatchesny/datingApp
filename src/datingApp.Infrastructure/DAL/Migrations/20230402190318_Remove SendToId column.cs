using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSendToIdcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SendToId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SendToId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SendToId",
                table: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SendToId",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SendToId",
                table: "Messages",
                column: "SendToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SendToId",
                table: "Messages",
                column: "SendToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
