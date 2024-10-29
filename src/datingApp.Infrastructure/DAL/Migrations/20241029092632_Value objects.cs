using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Valueobjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSettings_Lat_Lon",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "PreferredAgeFrom",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "PreferredAgeTo",
                table: "UserSettings");

            migrationBuilder.RenameColumn(
                name: "Lon",
                table: "UserSettings",
                newName: "Location_Lon");

            migrationBuilder.RenameColumn(
                name: "Lat",
                table: "UserSettings",
                newName: "Location_Lat");

            migrationBuilder.AlterColumn<double>(
                name: "Location_Lon",
                table: "UserSettings",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "Location_Lat",
                table: "UserSettings",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<int>(
                name: "PreferredAge_From",
                table: "UserSettings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreferredAge_To",
                table: "UserSettings",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredAge_From",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "PreferredAge_To",
                table: "UserSettings");

            migrationBuilder.RenameColumn(
                name: "Location_Lon",
                table: "UserSettings",
                newName: "Lon");

            migrationBuilder.RenameColumn(
                name: "Location_Lat",
                table: "UserSettings",
                newName: "Lat");

            migrationBuilder.AlterColumn<double>(
                name: "Lon",
                table: "UserSettings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Lat",
                table: "UserSettings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreferredAgeFrom",
                table: "UserSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreferredAgeTo",
                table: "UserSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_Lat_Lon",
                table: "UserSettings",
                columns: new[] { "Lat", "Lon" });
        }
    }
}
