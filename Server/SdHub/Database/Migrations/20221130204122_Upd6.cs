using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Embeddings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    About = table.Column<string>(type: "text", nullable: true),
                    SdVersion = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Embeddings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hypernets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    About = table.Column<string>(type: "text", nullable: true),
                    SdVersion = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hypernets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    About = table.Column<string>(type: "text", nullable: true),
                    SdVersion = table.Column<byte>(type: "smallint", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vaes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    About = table.Column<string>(type: "text", nullable: true),
                    SdVersion = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmbeddingVersions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmbeddingId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: true),
                    About = table.Column<string>(type: "text", nullable: true),
                    SourceLink = table.Column<string>(type: "text", nullable: true),
                    KnownNames = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbeddingVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmbeddingVersions_Embeddings_EmbeddingId",
                        column: x => x.EmbeddingId,
                        principalTable: "Embeddings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmbeddingVersions_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HypernetVersions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HypernetId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: true),
                    About = table.Column<string>(type: "text", nullable: true),
                    SourceLink = table.Column<string>(type: "text", nullable: true),
                    KnownNames = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HypernetVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HypernetVersions_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HypernetVersions_Hypernets_HypernetId",
                        column: x => x.HypernetId,
                        principalTable: "Hypernets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelVersions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModelId = table.Column<long>(type: "bigint", nullable: false),
                    HashV1 = table.Column<string>(type: "text", nullable: true),
                    CkptFileId = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: true),
                    About = table.Column<string>(type: "text", nullable: true),
                    SourceLink = table.Column<string>(type: "text", nullable: true),
                    KnownNames = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelVersions_Files_CkptFileId",
                        column: x => x.CkptFileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModelVersions_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VaeVersions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VaeId = table.Column<long>(type: "bigint", nullable: false),
                    ModelId = table.Column<long>(type: "bigint", nullable: true),
                    FileId = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: true),
                    About = table.Column<string>(type: "text", nullable: true),
                    SourceLink = table.Column<string>(type: "text", nullable: true),
                    KnownNames = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaeVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaeVersions_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VaeVersions_Vaes_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Vaes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GenerationSamples",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageId = table.Column<long>(type: "bigint", nullable: false),
                    ModelVersionId = table.Column<long>(type: "bigint", nullable: true),
                    HypernetVersionId = table.Column<long>(type: "bigint", nullable: true),
                    VaeVersionId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerationSamples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenerationSamples_HypernetVersions_HypernetVersionId",
                        column: x => x.HypernetVersionId,
                        principalTable: "HypernetVersions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GenerationSamples_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenerationSamples_ModelVersions_ModelVersionId",
                        column: x => x.ModelVersionId,
                        principalTable: "ModelVersions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GenerationSamples_VaeVersions_VaeVersionId",
                        column: x => x.VaeVersionId,
                        principalTable: "VaeVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingVersions_EmbeddingId",
                table: "EmbeddingVersions",
                column: "EmbeddingId");

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingVersions_FileId",
                table: "EmbeddingVersions",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_GenerationSamples_HypernetVersionId",
                table: "GenerationSamples",
                column: "HypernetVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_GenerationSamples_ImageId",
                table: "GenerationSamples",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_GenerationSamples_ModelVersionId",
                table: "GenerationSamples",
                column: "ModelVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_GenerationSamples_VaeVersionId",
                table: "GenerationSamples",
                column: "VaeVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_HypernetVersions_FileId",
                table: "HypernetVersions",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_HypernetVersions_HypernetId",
                table: "HypernetVersions",
                column: "HypernetId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelVersions_CkptFileId",
                table: "ModelVersions",
                column: "CkptFileId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelVersions_ModelId",
                table: "ModelVersions",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_VaeVersions_FileId",
                table: "VaeVersions",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_VaeVersions_ModelId",
                table: "VaeVersions",
                column: "ModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmbeddingVersions");

            migrationBuilder.DropTable(
                name: "GenerationSamples");

            migrationBuilder.DropTable(
                name: "Embeddings");

            migrationBuilder.DropTable(
                name: "HypernetVersions");

            migrationBuilder.DropTable(
                name: "ModelVersions");

            migrationBuilder.DropTable(
                name: "VaeVersions");

            migrationBuilder.DropTable(
                name: "Hypernets");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Vaes");
        }
    }
}
