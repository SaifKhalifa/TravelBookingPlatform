using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedByAdminFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Reviews_DeletedByAdminId",
                table: "Reviews",
                column: "DeletedByAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_DeletedByAdminId",
                table: "Reviews",
                column: "DeletedByAdminId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_DeletedByAdminId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_DeletedByAdminId",
                table: "Reviews");
        }
    }
}
