using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBYS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSignatureWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "DocumentSigners");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "DocumentSigners",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "RejectNote",
                table: "DocumentSigners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignedAt",
                table: "DocumentSigners",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentSignOrder",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WorkflowStatus",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectNote",
                table: "DocumentSigners");

            migrationBuilder.DropColumn(
                name: "SignedAt",
                table: "DocumentSigners");

            migrationBuilder.DropColumn(
                name: "CurrentSignOrder",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "WorkflowStatus",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "DocumentSigners",
                newName: "FullName");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "DocumentSigners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
