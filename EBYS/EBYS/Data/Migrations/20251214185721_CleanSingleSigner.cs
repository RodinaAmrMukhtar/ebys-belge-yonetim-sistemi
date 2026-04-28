using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBYS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CleanSingleSigner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentRecipients");

            migrationBuilder.DropTable(
                name: "DocumentSigners");

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
                name: "CurrentSignerOrder",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Dil",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "MajorVersiyonNo",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UreticiBelgesi",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UretimYeri",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "WorkflowStatus",
                table: "Documents",
                newName: "SignerUserId");

            migrationBuilder.AlterColumn<string>(
                name: "EditorContent",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SignerUserId",
                table: "Documents",
                newName: "WorkflowStatus");

            migrationBuilder.AlterColumn<string>(
                name: "EditorContent",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

            migrationBuilder.AddColumn<int>(
                name: "CurrentSignerOrder",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Dil",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MajorVersiyonNo",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateTable(
                name: "DocumentRecipients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentRecipients_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentSigners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    SignOrder = table.Column<int>(type: "int", nullable: false),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSigners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSigners_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRecipients_DocumentId",
                table: "DocumentRecipients",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSigners_DocumentId",
                table: "DocumentSigners",
                column: "DocumentId");
        }
    }
}
