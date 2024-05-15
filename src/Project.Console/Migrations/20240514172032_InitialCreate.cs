using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University.ConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Word = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tfidfs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomFileId = table.Column<int>(type: "INTEGER", nullable: false),
                    Repetation = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tfidfs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tfidfs_Files_CustomFileId",
                        column: x => x.CustomFileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TfidfModelWordTokenModel",
                columns: table => new
                {
                    TfId = table.Column<int>(type: "INTEGER", nullable: false),
                    WordTokenModelId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TfidfModelWordTokenModel", x => new { x.TfId, x.WordTokenModelId });
                    table.ForeignKey(
                        name: "FK_TfidfModelWordTokenModel_Tfidfs_TfId",
                        column: x => x.TfId,
                        principalTable: "Tfidfs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TfidfModelWordTokenModel_Words_WordTokenModelId",
                        column: x => x.WordTokenModelId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TfidfModelWordTokenModel_WordTokenModelId",
                table: "TfidfModelWordTokenModel",
                column: "WordTokenModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Tfidfs_CustomFileId",
                table: "Tfidfs",
                column: "CustomFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TfidfModelWordTokenModel");

            migrationBuilder.DropTable(
                name: "Tfidfs");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
