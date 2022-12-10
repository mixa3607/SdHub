using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class Upd13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmbeddingTagEntity_Embeddings_EmbeddingId",
                table: "EmbeddingTagEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_EmbeddingTagEntity_TagEntity_TagId",
                table: "EmbeddingTagEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_HypernetTagEntity_Hypernets_HypernetId",
                table: "HypernetTagEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_HypernetTagEntity_TagEntity_TagId",
                table: "HypernetTagEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelTagEntity_Models_ModelId",
                table: "ModelTagEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelTagEntity_TagEntity_TagId",
                table: "ModelTagEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_VaeTagEntity_TagEntity_TagId",
                table: "VaeTagEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_VaeTagEntity_Vaes_VaeId",
                table: "VaeTagEntity");

            migrationBuilder.DropIndex(
                name: "IX_GridImages_ImageId",
                table: "GridImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VaeTagEntity",
                table: "VaeTagEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagEntity",
                table: "TagEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModelTagEntity",
                table: "ModelTagEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HypernetTagEntity",
                table: "HypernetTagEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmbeddingTagEntity",
                table: "EmbeddingTagEntity");

            migrationBuilder.RenameTable(
                name: "VaeTagEntity",
                newName: "VaeTags");

            migrationBuilder.RenameTable(
                name: "TagEntity",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "ModelTagEntity",
                newName: "ModelTags");

            migrationBuilder.RenameTable(
                name: "HypernetTagEntity",
                newName: "HypernetTags");

            migrationBuilder.RenameTable(
                name: "EmbeddingTagEntity",
                newName: "EmbeddingTags");

            migrationBuilder.RenameColumn(
                name: "MaxArchiveSizeUpload",
                table: "UserPlans",
                newName: "ImagesPerGrid");

            migrationBuilder.RenameIndex(
                name: "IX_VaeTagEntity_VaeId",
                table: "VaeTags",
                newName: "IX_VaeTags_VaeId");

            migrationBuilder.RenameIndex(
                name: "IX_ModelTagEntity_ModelId",
                table: "ModelTags",
                newName: "IX_ModelTags_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_HypernetTagEntity_HypernetId",
                table: "HypernetTags",
                newName: "IX_HypernetTags_HypernetId");

            migrationBuilder.RenameIndex(
                name: "IX_EmbeddingTagEntity_EmbeddingId",
                table: "EmbeddingTags",
                newName: "IX_EmbeddingTags_EmbeddingId");

            migrationBuilder.AddColumn<string>(
                name: "DeleteReason",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "MaxImageSizeUpload",
                table: "UserPlans",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "GridsPerHour",
                table: "UserPlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "MaxGridArchiveSizeUpload",
                table: "UserPlans",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VaeTags",
                table: "VaeTags",
                columns: new[] { "TagId", "VaeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModelTags",
                table: "ModelTags",
                columns: new[] { "TagId", "ModelId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_HypernetTags",
                table: "HypernetTags",
                columns: new[] { "TagId", "HypernetId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmbeddingTags",
                table: "EmbeddingTags",
                columns: new[] { "TagId", "EmbeddingId" });

            migrationBuilder.CreateIndex(
                name: "IX_GridImages_ImageId",
                table: "GridImages",
                column: "ImageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmbeddingTags_Embeddings_EmbeddingId",
                table: "EmbeddingTags",
                column: "EmbeddingId",
                principalTable: "Embeddings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmbeddingTags_Tags_TagId",
                table: "EmbeddingTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HypernetTags_Hypernets_HypernetId",
                table: "HypernetTags",
                column: "HypernetId",
                principalTable: "Hypernets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HypernetTags_Tags_TagId",
                table: "HypernetTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelTags_Models_ModelId",
                table: "ModelTags",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelTags_Tags_TagId",
                table: "ModelTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VaeTags_Tags_TagId",
                table: "VaeTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VaeTags_Vaes_VaeId",
                table: "VaeTags",
                column: "VaeId",
                principalTable: "Vaes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmbeddingTags_Embeddings_EmbeddingId",
                table: "EmbeddingTags");

            migrationBuilder.DropForeignKey(
                name: "FK_EmbeddingTags_Tags_TagId",
                table: "EmbeddingTags");

            migrationBuilder.DropForeignKey(
                name: "FK_HypernetTags_Hypernets_HypernetId",
                table: "HypernetTags");

            migrationBuilder.DropForeignKey(
                name: "FK_HypernetTags_Tags_TagId",
                table: "HypernetTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelTags_Models_ModelId",
                table: "ModelTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelTags_Tags_TagId",
                table: "ModelTags");

            migrationBuilder.DropForeignKey(
                name: "FK_VaeTags_Tags_TagId",
                table: "VaeTags");

            migrationBuilder.DropForeignKey(
                name: "FK_VaeTags_Vaes_VaeId",
                table: "VaeTags");

            migrationBuilder.DropIndex(
                name: "IX_GridImages_ImageId",
                table: "GridImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VaeTags",
                table: "VaeTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModelTags",
                table: "ModelTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HypernetTags",
                table: "HypernetTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmbeddingTags",
                table: "EmbeddingTags");

            migrationBuilder.DropColumn(
                name: "DeleteReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GridsPerHour",
                table: "UserPlans");

            migrationBuilder.DropColumn(
                name: "MaxGridArchiveSizeUpload",
                table: "UserPlans");

            migrationBuilder.RenameTable(
                name: "VaeTags",
                newName: "VaeTagEntity");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "TagEntity");

            migrationBuilder.RenameTable(
                name: "ModelTags",
                newName: "ModelTagEntity");

            migrationBuilder.RenameTable(
                name: "HypernetTags",
                newName: "HypernetTagEntity");

            migrationBuilder.RenameTable(
                name: "EmbeddingTags",
                newName: "EmbeddingTagEntity");

            migrationBuilder.RenameColumn(
                name: "ImagesPerGrid",
                table: "UserPlans",
                newName: "MaxArchiveSizeUpload");

            migrationBuilder.RenameIndex(
                name: "IX_VaeTags_VaeId",
                table: "VaeTagEntity",
                newName: "IX_VaeTagEntity_VaeId");

            migrationBuilder.RenameIndex(
                name: "IX_ModelTags_ModelId",
                table: "ModelTagEntity",
                newName: "IX_ModelTagEntity_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_HypernetTags_HypernetId",
                table: "HypernetTagEntity",
                newName: "IX_HypernetTagEntity_HypernetId");

            migrationBuilder.RenameIndex(
                name: "IX_EmbeddingTags_EmbeddingId",
                table: "EmbeddingTagEntity",
                newName: "IX_EmbeddingTagEntity_EmbeddingId");

            migrationBuilder.AlterColumn<int>(
                name: "MaxImageSizeUpload",
                table: "UserPlans",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VaeTagEntity",
                table: "VaeTagEntity",
                columns: new[] { "TagId", "VaeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagEntity",
                table: "TagEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModelTagEntity",
                table: "ModelTagEntity",
                columns: new[] { "TagId", "ModelId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_HypernetTagEntity",
                table: "HypernetTagEntity",
                columns: new[] { "TagId", "HypernetId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmbeddingTagEntity",
                table: "EmbeddingTagEntity",
                columns: new[] { "TagId", "EmbeddingId" });

            migrationBuilder.CreateIndex(
                name: "IX_GridImages_ImageId",
                table: "GridImages",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmbeddingTagEntity_Embeddings_EmbeddingId",
                table: "EmbeddingTagEntity",
                column: "EmbeddingId",
                principalTable: "Embeddings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmbeddingTagEntity_TagEntity_TagId",
                table: "EmbeddingTagEntity",
                column: "TagId",
                principalTable: "TagEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HypernetTagEntity_Hypernets_HypernetId",
                table: "HypernetTagEntity",
                column: "HypernetId",
                principalTable: "Hypernets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HypernetTagEntity_TagEntity_TagId",
                table: "HypernetTagEntity",
                column: "TagId",
                principalTable: "TagEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelTagEntity_Models_ModelId",
                table: "ModelTagEntity",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelTagEntity_TagEntity_TagId",
                table: "ModelTagEntity",
                column: "TagId",
                principalTable: "TagEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VaeTagEntity_TagEntity_TagId",
                table: "VaeTagEntity",
                column: "TagId",
                principalTable: "TagEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VaeTagEntity_Vaes_VaeId",
                table: "VaeTagEntity",
                column: "VaeId",
                principalTable: "Vaes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
