using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBYS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEditorContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "EditorContent",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.AddColumn<int>(
                name: "MajorVersiyonNo",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                name: "BilgiEdinme",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "BirimHiyerarsi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Dil",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "EditorContent",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "GizlilikDerecesi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "KisiselBilgi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "MajorVersiyonNo",
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
