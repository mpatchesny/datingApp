using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddreferencetoPhotoinFileDto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Files_Id",
                table: "Photos");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Photos_Id",
                table: "Files",
                column: "Id",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Photos_Id",
                table: "Files");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Files_Id",
                table: "Photos",
                column: "Id",
                principalTable: "Files",
                principalColumn: "Id");
        }
    }
}
