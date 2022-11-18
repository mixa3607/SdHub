using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_FileStores_StorageName",
                table: "Files");

            migrationBuilder.DropTable(
                name: "EmailCheckRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileStores",
                table: "FileStores");

            migrationBuilder.RenameTable(
                name: "FileStores",
                newName: "FileStorages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileStorages",
                table: "FileStorages",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_FileStorages_StorageName",
                table: "Files",
                column: "StorageName",
                principalTable: "FileStorages",
                principalColumn: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_FileStorages_StorageName",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileStorages",
                table: "FileStorages");

            migrationBuilder.RenameTable(
                name: "FileStorages",
                newName: "FileStores");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileStores",
                table: "FileStores",
                column: "Name");

            migrationBuilder.CreateTable(
                name: "EmailCheckRules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Template = table.Column<string>(type: "text", nullable: false),
                    TrustLevel = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailCheckRules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailCheckRules_Template",
                table: "EmailCheckRules",
                column: "Template",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_FileStores_StorageName",
                table: "Files",
                column: "StorageName",
                principalTable: "FileStores",
                principalColumn: "Name");
        }
    }
}
