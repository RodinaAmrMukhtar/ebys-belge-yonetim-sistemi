using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBYS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Documents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Documents",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Documents");
        }
    }
}
