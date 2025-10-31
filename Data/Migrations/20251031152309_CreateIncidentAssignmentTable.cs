using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftOfTheGiver_Foundation.Migrations
{
    /// <inheritdoc />
    public partial class CreateIncidentAssignmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncidentAssignments",
                columns: table => new
                {
                    IncidentAssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisasterReportId = table.Column<int>(type: "int", nullable: false),
                    VolunteerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentAssignments", x => x.IncidentAssignmentId);
                    table.ForeignKey(
                        name: "FK_IncidentAssignments_DisasterReports_DisasterReportId",
                        column: x => x.DisasterReportId,
                        principalTable: "DisasterReports",
                        principalColumn: "DisasterReportId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentAssignments_Volunteers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "Volunteers",
                        principalColumn: "VolunteerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentAssignments_DisasterReportId",
                table: "IncidentAssignments",
                column: "DisasterReportId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentAssignments_VolunteerId",
                table: "IncidentAssignments",
                column: "VolunteerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentAssignments");
        }
    }
}
