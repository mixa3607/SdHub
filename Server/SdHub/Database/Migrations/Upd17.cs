using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModelVersions_Files_CkptFileId",
                table: "ModelVersions");

            migrationBuilder.DropIndex(
                name: "IX_ModelVersions_CkptFileId",
                table: "ModelVersions");

            migrationBuilder.DropColumn(
                name: "CkptFileId",
                table: "ModelVersions");

            migrationBuilder.DropColumn(
                name: "HashV1",
                table: "ModelVersions");

            migrationBuilder.CreateTable(
                name: "ModelVersionFiles",
                columns: table => new
                {
                    ModelVersionId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    ModelHashV1 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelVersionFiles", x => new { x.FileId, x.ModelVersionId });
                    table.ForeignKey(
                        name: "FK_ModelVersionFiles_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModelVersionFiles_ModelVersions_ModelVersionId",
                        column: x => x.ModelVersionId,
                        principalTable: "ModelVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelVersionFiles_ModelVersionId",
                table: "ModelVersionFiles",
                column: "ModelVersionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelVersionFiles");

            migrationBuilder.AddColumn<long>(
                name: "CkptFileId",
                table: "ModelVersions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HashV1",
                table: "ModelVersions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelVersions_CkptFileId",
                table: "ModelVersions",
                column: "CkptFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModelVersions_Files_CkptFileId",
                table: "ModelVersions",
                column: "CkptFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }
    }
}
