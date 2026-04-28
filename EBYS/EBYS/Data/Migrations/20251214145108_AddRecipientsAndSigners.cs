using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBYS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipientsAndSigners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "DocumentRecipients");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "DocumentRecipients");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "DocumentRecipients",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "DocumentRecipients",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "FromUserId",
                table: "DocumentRecipients",
                newName: "DepartmentName");

            migrationBuilder.CreateTable(
                name: "DocumentSigners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignOrder = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_DocumentSigners_DocumentId",
                table: "DocumentSigners",
                column: "DocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentSigners");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "DocumentRecipients",
                newName: "ToUserId");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "DocumentRecipients",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "DepartmentName",
                table: "DocumentRecipients",
                newName: "FromUserId");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "DocumentRecipients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "DocumentRecipients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
