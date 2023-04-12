using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Removedafultvalfrommessageaddcolumnstomatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDisplayed",
                table: "Messages",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDisplayed",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }
    }
}
