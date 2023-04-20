using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Compositeindexonswipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Swipes_SwippedById",
                table: "Swipes");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwippedById_SwippedWhoId",
                table: "Swipes",
                columns: new[] { "SwippedById", "SwippedWhoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Swipes_SwippedById_SwippedWhoId",
                table: "Swipes");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwippedById",
                table: "Swipes",
                column: "SwippedById");
        }
    }
}
