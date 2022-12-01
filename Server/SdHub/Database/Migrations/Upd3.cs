using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_PathOnStorage_StorageName",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Dirs_PathOnStorage_StorageName",
                table: "Dirs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Files_PathOnStorage_StorageName",
                table: "Files",
                columns: new[] { "PathOnStorage", "StorageName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dirs_PathOnStorage_StorageName",
                table: "Dirs",
                columns: new[] { "PathOnStorage", "StorageName" },
                unique: true);
        }
    }
}
