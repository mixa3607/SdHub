using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TagEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmbeddingTagEntity",
                columns: table => new
                {
                    TagId = table.Column<long>(type: "bigint", nullable: false),
                    EmbeddingId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbeddingTagEntity", x => new { x.TagId, x.EmbeddingId });
                    table.ForeignKey(
                        name: "FK_EmbeddingTagEntity_Embeddings_EmbeddingId",
                        column: x => x.EmbeddingId,
                        principalTable: "Embeddings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmbeddingTagEntity_TagEntity_TagId",
                        column: x => x.TagId,
                        principalTable: "TagEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HypernetTagEntity",
                columns: table => new
                {
                    TagId = table.Column<long>(type: "bigint", nullable: false),
                    HypernetId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HypernetTagEntity", x => new { x.TagId, x.HypernetId });
                    table.ForeignKey(
                        name: "FK_HypernetTagEntity_Hypernets_HypernetId",
                        column: x => x.HypernetId,
                        principalTable: "Hypernets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HypernetTagEntity_TagEntity_TagId",
                        column: x => x.TagId,
                        principalTable: "TagEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelTagEntity",
                columns: table => new
                {
                    TagId = table.Column<long>(type: "bigint", nullable: false),
                    ModelId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelTagEntity", x => new { x.TagId, x.ModelId });
                    table.ForeignKey(
                        name: "FK_ModelTagEntity_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModelTagEntity_TagEntity_TagId",
                        column: x => x.TagId,
                        principalTable: "TagEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VaeTagEntity",
                columns: table => new
                {
                    TagId = table.Column<long>(type: "bigint", nullable: false),
                    VaeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaeTagEntity", x => new { x.TagId, x.VaeId });
                    table.ForeignKey(
                        name: "FK_VaeTagEntity_TagEntity_TagId",
                        column: x => x.TagId,
                        principalTable: "TagEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VaeTagEntity_Vaes_VaeId",
                        column: x => x.VaeId,
                        principalTable: "Vaes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingTagEntity_EmbeddingId",
                table: "EmbeddingTagEntity",
                column: "EmbeddingId");

            migrationBuilder.CreateIndex(
                name: "IX_HypernetTagEntity_HypernetId",
                table: "HypernetTagEntity",
                column: "HypernetId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelTagEntity_ModelId",
                table: "ModelTagEntity",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_VaeTagEntity_VaeId",
                table: "VaeTagEntity",
                column: "VaeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmbeddingTagEntity");

            migrationBuilder.DropTable(
                name: "HypernetTagEntity");

            migrationBuilder.DropTable(
                name: "ModelTagEntity");

            migrationBuilder.DropTable(
                name: "VaeTagEntity");

            migrationBuilder.DropTable(
                name: "TagEntity");
        }
    }
}
