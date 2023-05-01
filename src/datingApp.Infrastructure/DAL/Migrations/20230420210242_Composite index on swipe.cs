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
                name: "IX_Swipes_SwipedById",
                table: "Swipes");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwipedById_SwipedWhoId",
                table: "Swipes",
                columns: new[] { "SwipedById", "SwipedWhoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Swipes_SwipedById_SwipedWhoId",
                table: "Swipes");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwipedById",
                table: "Swipes",
                column: "SwipedById");
        }
    }
}
