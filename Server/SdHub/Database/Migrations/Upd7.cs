using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EmbeddingVersionId",
                table: "GenerationSamples",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Trigger",
                table: "Embeddings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_GenerationSamples_EmbeddingVersionId",
                table: "GenerationSamples",
                column: "EmbeddingVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationSamples_EmbeddingVersions_EmbeddingVersionId",
                table: "GenerationSamples",
                column: "EmbeddingVersionId",
                principalTable: "EmbeddingVersions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenerationSamples_EmbeddingVersions_EmbeddingVersionId",
                table: "GenerationSamples");

            migrationBuilder.DropIndex(
                name: "IX_GenerationSamples_EmbeddingVersionId",
                table: "GenerationSamples");

            migrationBuilder.DropColumn(
                name: "EmbeddingVersionId",
                table: "GenerationSamples");

            migrationBuilder.DropColumn(
                name: "Trigger",
                table: "Embeddings");
        }
    }
}
