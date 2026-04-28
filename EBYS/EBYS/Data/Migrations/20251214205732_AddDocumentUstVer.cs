using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBYS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentUstVer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BelgeKategorisi",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "BelgeTarihi",
                table: "Documents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BelgeTuru",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "BilaTarih",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
                name: "Dil",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GizlilikDerecesi",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "KisiselBilgi",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MayorVersiyonNo",
                table: "Documents",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "UreticiBelgesi",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UretimYeri",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ZorunluHal",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BelgeKategorisi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "BelgeTarihi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "BelgeTuru",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "BilaTarih",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "BilgiEdinme",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "BirimHiyerarsi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Dil",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "GizlilikDerecesi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "KisiselBilgi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "MayorVersiyonNo",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "OlaganustuDurum",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "TelifYasasi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UreticiBelgesi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UretimYeri",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ZorunluHal",
                table: "Documents");
        }
    }
}
