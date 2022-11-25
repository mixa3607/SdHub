using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CompressedImageId",
                table: "Images",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_CompressedImageId",
                table: "Images",
                column: "CompressedImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Files_CompressedImageId",
                table: "Images",
                column: "CompressedImageId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Files_CompressedImageId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_CompressedImageId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "CompressedImageId",
                table: "Images");
        }
    }
}
