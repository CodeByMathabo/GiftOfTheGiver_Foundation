using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftOfTheGiver_Foundation.Migrations
{
    /// <inheritdoc />
    public partial class AddResolutionNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AttendedDate",
                table: "IncidentAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionNotes",
                table: "DisasterReports",
                type: "nvarchar(MAX)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttendedDate",
                table: "IncidentAssignments");

            migrationBuilder.DropColumn(
                name: "ResolutionNotes",
                table: "DisasterReports");
        }
    }
}
