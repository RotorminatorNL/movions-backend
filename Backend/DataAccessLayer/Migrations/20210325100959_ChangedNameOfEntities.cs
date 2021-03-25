using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class ChangedNameOfEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrewRoles_Movies_MovieDTOID",
                table: "CrewRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_CrewRoles_Persons_PersonDTOID",
                table: "CrewRoles");

            migrationBuilder.DropTable(
                name: "CompanyDTOMovieDTO");

            migrationBuilder.DropTable(
                name: "GenreDTOMovieDTO");

            migrationBuilder.DropTable(
                name: "LanguageDTOMovieDTO");

            migrationBuilder.RenameColumn(
                name: "PersonDTOID",
                table: "CrewRoles",
                newName: "PersonID");

            migrationBuilder.RenameColumn(
                name: "MovieDTOID",
                table: "CrewRoles",
                newName: "MovieID");

            migrationBuilder.RenameIndex(
                name: "IX_CrewRoles_PersonDTOID",
                table: "CrewRoles",
                newName: "IX_CrewRoles_PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_CrewRoles_MovieDTOID",
                table: "CrewRoles",
                newName: "IX_CrewRoles_MovieID");

            migrationBuilder.CreateTable(
                name: "CompanyMovie",
                columns: table => new
                {
                    CompaniesID = table.Column<int>(type: "int", nullable: false),
                    MoviesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyMovie", x => new { x.CompaniesID, x.MoviesID });
                    table.ForeignKey(
                        name: "FK_CompanyMovie_Companies_CompaniesID",
                        column: x => x.CompaniesID,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyMovie_Movies_MoviesID",
                        column: x => x.MoviesID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreMovie",
                columns: table => new
                {
                    GenresID = table.Column<int>(type: "int", nullable: false),
                    MoviesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreMovie", x => new { x.GenresID, x.MoviesID });
                    table.ForeignKey(
                        name: "FK_GenreMovie_Genres_GenresID",
                        column: x => x.GenresID,
                        principalTable: "Genres",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreMovie_Movies_MoviesID",
                        column: x => x.MoviesID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LanguageMovie",
                columns: table => new
                {
                    LanguagesID = table.Column<int>(type: "int", nullable: false),
                    MoviesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageMovie", x => new { x.LanguagesID, x.MoviesID });
                    table.ForeignKey(
                        name: "FK_LanguageMovie_Languages_LanguagesID",
                        column: x => x.LanguagesID,
                        principalTable: "Languages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LanguageMovie_Movies_MoviesID",
                        column: x => x.MoviesID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMovie_MoviesID",
                table: "CompanyMovie",
                column: "MoviesID");

            migrationBuilder.CreateIndex(
                name: "IX_GenreMovie_MoviesID",
                table: "GenreMovie",
                column: "MoviesID");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageMovie_MoviesID",
                table: "LanguageMovie",
                column: "MoviesID");

            migrationBuilder.AddForeignKey(
                name: "FK_CrewRoles_Movies_MovieID",
                table: "CrewRoles",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CrewRoles_Persons_PersonID",
                table: "CrewRoles",
                column: "PersonID",
                principalTable: "Persons",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrewRoles_Movies_MovieID",
                table: "CrewRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_CrewRoles_Persons_PersonID",
                table: "CrewRoles");

            migrationBuilder.DropTable(
                name: "CompanyMovie");

            migrationBuilder.DropTable(
                name: "GenreMovie");

            migrationBuilder.DropTable(
                name: "LanguageMovie");

            migrationBuilder.RenameColumn(
                name: "PersonID",
                table: "CrewRoles",
                newName: "PersonDTOID");

            migrationBuilder.RenameColumn(
                name: "MovieID",
                table: "CrewRoles",
                newName: "MovieDTOID");

            migrationBuilder.RenameIndex(
                name: "IX_CrewRoles_PersonID",
                table: "CrewRoles",
                newName: "IX_CrewRoles_PersonDTOID");

            migrationBuilder.RenameIndex(
                name: "IX_CrewRoles_MovieID",
                table: "CrewRoles",
                newName: "IX_CrewRoles_MovieDTOID");

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

            migrationBuilder.AddForeignKey(
                name: "FK_CrewRoles_Movies_MovieDTOID",
                table: "CrewRoles",
                column: "MovieDTOID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CrewRoles_Persons_PersonDTOID",
                table: "CrewRoles",
                column: "PersonDTOID",
                principalTable: "Persons",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
