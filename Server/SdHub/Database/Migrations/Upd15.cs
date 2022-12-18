using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenerationSamples_ModelVersions_ModelVersionId",
                table: "GenerationSamples");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelVersions_Files_CkptFileId",
                table: "ModelVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelVersions_Models_ModelId",
                table: "ModelVersions");

            migrationBuilder.DropColumn(
                name: "SourceLink",
                table: "ModelVersions");

            migrationBuilder.AlterColumn<long>(
                name: "CkptFileId",
                table: "ModelVersions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationSamples_ModelVersions_ModelVersionId",
                table: "GenerationSamples",
                column: "ModelVersionId",
                principalTable: "ModelVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelVersions_Files_CkptFileId",
                table: "ModelVersions",
                column: "CkptFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ModelVersions_Models_ModelId",
                table: "ModelVersions",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenerationSamples_ModelVersions_ModelVersionId",
                table: "GenerationSamples");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelVersions_Files_CkptFileId",
                table: "ModelVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelVersions_Models_ModelId",
                table: "ModelVersions");

            migrationBuilder.AlterColumn<long>(
                name: "CkptFileId",
                table: "ModelVersions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceLink",
                table: "ModelVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationSamples_ModelVersions_ModelVersionId",
                table: "GenerationSamples",
                column: "ModelVersionId",
                principalTable: "ModelVersions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ModelVersions_Files_CkptFileId",
                table: "ModelVersions",
                column: "CkptFileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelVersions_Models_ModelId",
                table: "ModelVersions",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
