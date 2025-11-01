using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftOfTheGiver_Foundation.Migrations
{
    /// <inheritdoc />
    public partial class AddVolunteerEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VolunteerEmail",
                table: "Volunteers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VolunteerEmail",
                table: "Volunteers");
        }
    }
}
