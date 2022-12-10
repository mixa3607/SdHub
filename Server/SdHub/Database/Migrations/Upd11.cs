using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumImages_Grids_GridId",
                table: "AlbumImages");

            migrationBuilder.DropForeignKey(
                name: "FK_AlbumImages_Images_ImageId",
                table: "AlbumImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AlbumImages",
                table: "AlbumImages");

            migrationBuilder.AlterColumn<long>(
                name: "GridId",
                table: "AlbumImages",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ImageId",
                table: "AlbumImages",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "AlbumImages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlbumImages",
                table: "AlbumImages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumImages_AlbumId_ImageId_GridId",
                table: "AlbumImages",
                columns: new[] { "AlbumId", "ImageId", "GridId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumImages_Grids_GridId",
                table: "AlbumImages",
                column: "GridId",
                principalTable: "Grids",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumImages_Images_ImageId",
                table: "AlbumImages",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumImages_Grids_GridId",
                table: "AlbumImages");

            migrationBuilder.DropForeignKey(
                name: "FK_AlbumImages_Images_ImageId",
                table: "AlbumImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AlbumImages",
                table: "AlbumImages");

            migrationBuilder.DropIndex(
                name: "IX_AlbumImages_AlbumId_ImageId_GridId",
                table: "AlbumImages");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AlbumImages");

            migrationBuilder.AlterColumn<long>(
                name: "ImageId",
                table: "AlbumImages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "GridId",
                table: "AlbumImages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlbumImages",
                table: "AlbumImages",
                columns: new[] { "AlbumId", "ImageId", "GridId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumImages_Grids_GridId",
                table: "AlbumImages",
                column: "GridId",
                principalTable: "Grids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumImages_Images_ImageId",
                table: "AlbumImages",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
