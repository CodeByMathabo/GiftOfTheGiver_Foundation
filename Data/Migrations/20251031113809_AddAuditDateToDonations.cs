using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftOfTheGiver_Foundation.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditDateToDonations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "ResourceDonation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "MonetaryDonation",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "ResourceDonation");

            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "MonetaryDonation");
        }
    }
}
