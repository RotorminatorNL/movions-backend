using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class ChangedRelationMovieLanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieLanguages");

            migrationBuilder.AddColumn<int>(
                name: "LanguageID",
                table: "Movies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_LanguageID",
                table: "Movies",
                column: "LanguageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Languages_LanguageID",
                table: "Movies",
                column: "LanguageID",
                principalTable: "Languages",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Languages_LanguageID",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_LanguageID",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "Movies");

            migrationBuilder.CreateTable(
                name: "MovieLanguages",
                columns: table => new
                {
                    MovieID = table.Column<int>(type: "int", nullable: false),
                    LanguageID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieLanguages", x => new { x.MovieID, x.LanguageID });
                    table.ForeignKey(
                        name: "FK_MovieLanguages_Languages_LanguageID",
                        column: x => x.LanguageID,
                        principalTable: "Languages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieLanguages_Movies_MovieID",
                        column: x => x.MovieID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieLanguages_LanguageID",
                table: "MovieLanguages",
                column: "LanguageID");
        }
    }
}
