using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Matchdetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Users_UserId1",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Users_UserId2",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Matches_MatchId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MatchId_CreatedAt",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Matches_UserId1_UserId2_CreatedAt",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_UserId2",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "IsDisplayedByUser1",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "IsDisplayedByUser2",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "UserId2",
                table: "Matches");

            migrationBuilder.AlterColumn<Guid>(
                name: "MatchId",
                table: "Messages",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "MatchDetailId",
                table: "Messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MatchDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDisplayed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchDetail_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedAt",
                table: "Messages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MatchDetailId",
                table: "Messages",
                column: "MatchDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MatchId",
                table: "Messages",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchDetail_MatchId",
                table: "MatchDetail",
                column: "MatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_MatchDetail_MatchDetailId",
                table: "Messages",
                column: "MatchDetailId",
                principalTable: "MatchDetail",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Matches_MatchId",
                table: "Messages",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_MatchDetail_MatchDetailId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Matches_MatchId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "MatchDetail");

            migrationBuilder.DropIndex(
                name: "IX_Messages_CreatedAt",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MatchDetailId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MatchId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MatchDetailId",
                table: "Messages");

            migrationBuilder.AlterColumn<Guid>(
                name: "MatchId",
                table: "Messages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisplayedByUser1",
                table: "Matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisplayedByUser2",
                table: "Matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Matches",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId2",
                table: "Matches",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MatchId_CreatedAt",
                table: "Messages",
                columns: new[] { "MatchId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_UserId1_UserId2_CreatedAt",
                table: "Matches",
                columns: new[] { "UserId1", "UserId2", "CreatedAt" });

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
                name: "FK_Messages_Matches_MatchId",
                table: "Messages",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
