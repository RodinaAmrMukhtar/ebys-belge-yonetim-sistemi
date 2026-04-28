using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBYS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "DocumentAttachments");

            migrationBuilder.RenameColumn(
                name: "UserFullName",
                table: "DocumentSigners",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "UploadedByUserId",
                table: "DocumentAttachments",
                newName: "StoredFileName");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "DocumentAttachments",
                newName: "FileSize");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "DocumentSigners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "DocumentSigners");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "DocumentSigners",
                newName: "UserFullName");

            migrationBuilder.RenameColumn(
                name: "StoredFileName",
                table: "DocumentAttachments",
                newName: "UploadedByUserId");

            migrationBuilder.RenameColumn(
                name: "FileSize",
                table: "DocumentAttachments",
                newName: "Size");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "DocumentAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
