using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grids",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortToken = table.Column<string>(type: "text", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    XTiles = table.Column<int>(type: "integer", nullable: false),
                    YTiles = table.Column<int>(type: "integer", nullable: false),
                    XValues = table.Column<List<string>>(type: "text[]", nullable: false),
                    YValues = table.Column<List<string>>(type: "text[]", nullable: false),
                    MinLayer = table.Column<int>(type: "integer", nullable: false),
                    MaxLayer = table.Column<int>(type: "integer", nullable: false),
                    ThumbImageId = table.Column<long>(type: "bigint", nullable: true),
                    LayersDirectoryId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grids_Dirs_LayersDirectoryId",
                        column: x => x.LayersDirectoryId,
                        principalTable: "Dirs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Grids_Files_ThumbImageId",
                        column: x => x.ThumbImageId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Grids_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GridImages",
                columns: table => new
                {
                    GridId = table.Column<long>(type: "bigint", nullable: false),
                    ImageId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GridImages", x => new { x.GridId, x.ImageId });
                    table.ForeignKey(
                        name: "FK_GridImages_Grids_GridId",
                        column: x => x.GridId,
                        principalTable: "Grids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GridImages_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GridImages_ImageId",
                table: "GridImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Grids_LayersDirectoryId",
                table: "Grids",
                column: "LayersDirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Grids_OwnerId",
                table: "Grids",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Grids_ThumbImageId",
                table: "Grids",
                column: "ThumbImageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GridImages");

            migrationBuilder.DropTable(
                name: "Grids");
        }
    }
}
