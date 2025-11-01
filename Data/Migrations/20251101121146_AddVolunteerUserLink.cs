using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftOfTheGiver_Foundation.Migrations
{
    /// <inheritdoc />
    public partial class AddVolunteerUserLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Volunteers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_ApplicationUserId",
                table: "Volunteers",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_AspNetUsers_ApplicationUserId",
                table: "Volunteers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_AspNetUsers_ApplicationUserId",
                table: "Volunteers");

            migrationBuilder.DropIndex(
                name: "IX_Volunteers_ApplicationUserId",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Volunteers");
        }
    }
}
