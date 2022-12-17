using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AlbumImages_AlbumId_ImageId_GridId",
                table: "AlbumImages");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Tags",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Value",
                table: "Tags",
                column: "Value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_ShortToken",
                table: "Images",
                column: "ShortToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Grids_ShortToken",
                table: "Grids",
                column: "ShortToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Albums_ShortToken",
                table: "Albums",
                column: "ShortToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlbumImages_AlbumId_GridId",
                table: "AlbumImages",
                columns: new[] { "AlbumId", "GridId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlbumImages_AlbumId_ImageId",
                table: "AlbumImages",
                columns: new[] { "AlbumId", "ImageId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Value",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Images_ShortToken",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Grids_ShortToken",
                table: "Grids");

            migrationBuilder.DropIndex(
                name: "IX_Albums_ShortToken",
                table: "Albums");

            migrationBuilder.DropIndex(
                name: "IX_AlbumImages_AlbumId_GridId",
                table: "AlbumImages");

            migrationBuilder.DropIndex(
                name: "IX_AlbumImages_AlbumId_ImageId",
                table: "AlbumImages");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Tags",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumImages_AlbumId_ImageId_GridId",
                table: "AlbumImages",
                columns: new[] { "AlbumId", "ImageId", "GridId" },
                unique: true);
        }
    }
}
