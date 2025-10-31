using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftOfTheGiver_Foundation.Migrations
{
    /// <inheritdoc />
    public partial class CreateDisasterReportsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisasterReports",
                columns: table => new
                {
                    DisasterReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UrgencyLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisasterReports", x => x.DisasterReportId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisasterReports");
        }
    }
}
