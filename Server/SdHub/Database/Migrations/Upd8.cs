using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaeVersions_Vaes_ModelId",
                table: "VaeVersions");

            migrationBuilder.DropIndex(
                name: "IX_VaeVersions_ModelId",
                table: "VaeVersions");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "VaeVersions");

            migrationBuilder.CreateIndex(
                name: "IX_VaeVersions_VaeId",
                table: "VaeVersions",
                column: "VaeId");

            migrationBuilder.AddForeignKey(
                name: "FK_VaeVersions_Vaes_VaeId",
                table: "VaeVersions",
                column: "VaeId",
                principalTable: "Vaes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaeVersions_Vaes_VaeId",
                table: "VaeVersions");

            migrationBuilder.DropIndex(
                name: "IX_VaeVersions_VaeId",
                table: "VaeVersions");

            migrationBuilder.AddColumn<long>(
                name: "ModelId",
                table: "VaeVersions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaeVersions_ModelId",
                table: "VaeVersions",
                column: "ModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_VaeVersions_Vaes_ModelId",
                table: "VaeVersions",
                column: "ModelId",
                principalTable: "Vaes",
                principalColumn: "Id");
        }
    }
}
