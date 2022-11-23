using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SdHub.Database.Migrations
{
    public partial class UsrMgmt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EmailConfirmationLastSend",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EmailConfirmedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailNormalized",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Dirs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    StorageName = table.Column<string>(type: "text", nullable: true),
                    PathOnStorage = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dirs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dirs_FileStorages_StorageName",
                        column: x => x.StorageName,
                        principalTable: "FileStorages",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "TempCodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodeType = table.Column<int>(type: "integer", nullable: false),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    MaxAttempts = table.Column<int>(type: "integer", nullable: false),
                    CurrAttempts = table.Column<int>(type: "integer", nullable: false),
                    ExpiredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempCodes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dirs_PathOnStorage_StorageName",
                table: "Dirs",
                columns: new[] { "PathOnStorage", "StorageName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dirs_StorageName",
                table: "Dirs",
                column: "StorageName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dirs");

            migrationBuilder.DropTable(
                name: "TempCodes");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailConfirmationLastSend",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailConfirmedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailNormalized",
                table: "Users");
        }
    }
}
