using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddedLists2CGL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Movies_MovieDTOID",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Movies_MovieDTOID",
                table: "Genres");

            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Movies_MovieDTOID",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_MovieDTOID",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Genres_MovieDTOID",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Companies_MovieDTOID",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MovieDTOID",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "MovieDTOID",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "MovieDTOID",
                table: "Companies");

            migrationBuilder.CreateTable(
                name: "CompanyDTOMovieDTO",
                columns: table => new
                {
                    CompaniesID = table.Column<int>(type: "int", nullable: false),
                    MoviesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyDTOMovieDTO", x => new { x.CompaniesID, x.MoviesID });
                    table.ForeignKey(
                        name: "FK_CompanyDTOMovieDTO_Companies_CompaniesID",
                        column: x => x.CompaniesID,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyDTOMovieDTO_Movies_MoviesID",
                        column: x => x.MoviesID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreDTOMovieDTO",
                columns: table => new
                {
                    GenresID = table.Column<int>(type: "int", nullable: false),
                    MoviesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreDTOMovieDTO", x => new { x.GenresID, x.MoviesID });
                    table.ForeignKey(
                        name: "FK_GenreDTOMovieDTO_Genres_GenresID",
                        column: x => x.GenresID,
                        principalTable: "Genres",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreDTOMovieDTO_Movies_MoviesID",
                        column: x => x.MoviesID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LanguageDTOMovieDTO",
                columns: table => new
                {
                    LanguagesID = table.Column<int>(type: "int", nullable: false),
                    MoviesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageDTOMovieDTO", x => new { x.LanguagesID, x.MoviesID });
                    table.ForeignKey(
                        name: "FK_LanguageDTOMovieDTO_Languages_LanguagesID",
                        column: x => x.LanguagesID,
                        principalTable: "Languages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LanguageDTOMovieDTO_Movies_MoviesID",
                        column: x => x.MoviesID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDTOMovieDTO_MoviesID",
                table: "CompanyDTOMovieDTO",
                column: "MoviesID");

            migrationBuilder.CreateIndex(
                name: "IX_GenreDTOMovieDTO_MoviesID",
                table: "GenreDTOMovieDTO",
                column: "MoviesID");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageDTOMovieDTO_MoviesID",
                table: "LanguageDTOMovieDTO",
                column: "MoviesID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyDTOMovieDTO");

            migrationBuilder.DropTable(
                name: "GenreDTOMovieDTO");

            migrationBuilder.DropTable(
                name: "LanguageDTOMovieDTO");

            migrationBuilder.AddColumn<int>(
                name: "MovieDTOID",
                table: "Languages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MovieDTOID",
                table: "Genres",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MovieDTOID",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_MovieDTOID",
                table: "Languages",
                column: "MovieDTOID");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_MovieDTOID",
                table: "Genres",
                column: "MovieDTOID");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_MovieDTOID",
                table: "Companies",
                column: "MovieDTOID");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Movies_MovieDTOID",
                table: "Companies",
                column: "MovieDTOID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Movies_MovieDTOID",
                table: "Genres",
                column: "MovieDTOID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Movies_MovieDTOID",
                table: "Languages",
                column: "MovieDTOID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
