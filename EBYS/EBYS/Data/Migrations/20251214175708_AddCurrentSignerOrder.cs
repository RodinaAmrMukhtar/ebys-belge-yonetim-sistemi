using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBYS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentSignerOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectNote",
                table: "DocumentSigners");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "DocumentSigners");

            migrationBuilder.DropColumn(
                name: "BelgeKategorisi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "BilgiEdinme",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "BirimHiyerarsi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "GizlilikDerecesi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "KisiselBilgi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "OlaganustuDurum",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "TelifYasasi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ZorunluHal",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "DocumentAttachments");

            migrationBuilder.RenameColumn(
                name: "CurrentSignOrder",
                table: "Documents",
                newName: "CurrentSignerOrder");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "DocumentSigners",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "DocumentRecipients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "DocumentSigners");

            migrationBuilder.RenameColumn(
                name: "CurrentSignerOrder",
                table: "Documents",
                newName: "CurrentSignOrder");

            migrationBuilder.AddColumn<string>(
                name: "RejectNote",
                table: "DocumentSigners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "DocumentSigners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BelgeKategorisi",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "BilgiEdinme",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BirimHiyerarsi",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "GizlilikDerecesi",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "KisiselBilgi",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OlaganustuDurum",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TelifYasasi",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ZorunluHal",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "DocumentRecipients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "DocumentAttachments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
