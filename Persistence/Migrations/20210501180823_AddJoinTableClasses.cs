using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddJoinTableClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMovie_Companies_CompaniesID",
                table: "CompanyMovie");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMovie_Movies_MoviesID",
                table: "CompanyMovie");

            migrationBuilder.DropForeignKey(
                name: "FK_GenreMovie_Genres_GenresID",
                table: "GenreMovie");

            migrationBuilder.DropForeignKey(
                name: "FK_GenreMovie_Movies_MoviesID",
                table: "GenreMovie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GenreMovie",
                table: "GenreMovie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyMovie",
                table: "CompanyMovie");

            migrationBuilder.RenameColumn(
                name: "MoviesID",
                table: "GenreMovie",
                newName: "MovieID");

            migrationBuilder.RenameColumn(
                name: "GenresID",
                table: "GenreMovie",
                newName: "GenreID");

            migrationBuilder.RenameIndex(
                name: "IX_GenreMovie_MoviesID",
                table: "GenreMovie",
                newName: "IX_GenreMovie_MovieID");

            migrationBuilder.RenameColumn(
                name: "MoviesID",
                table: "CompanyMovie",
                newName: "MovieID");

            migrationBuilder.RenameColumn(
                name: "CompaniesID",
                table: "CompanyMovie",
                newName: "CompanyID");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMovie_MoviesID",
                table: "CompanyMovie",
                newName: "IX_CompanyMovie_MovieID");

            migrationBuilder.AddColumn<int>(
                name: "GenreMovieID",
                table: "GenreMovie",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "CompanyMovieID",
                table: "CompanyMovie",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GenreMovie",
                table: "GenreMovie",
                column: "GenreMovieID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyMovie",
                table: "CompanyMovie",
                column: "CompanyMovieID");

            migrationBuilder.CreateIndex(
                name: "IX_GenreMovie_GenreID",
                table: "GenreMovie",
                column: "GenreID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMovie_CompanyID",
                table: "CompanyMovie",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMovie_Companies_CompanyID",
                table: "CompanyMovie",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMovie_Movies_MovieID",
                table: "CompanyMovie",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GenreMovie_Genres_GenreID",
                table: "GenreMovie",
                column: "GenreID",
                principalTable: "Genres",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GenreMovie_Movies_MovieID",
                table: "GenreMovie",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMovie_Companies_CompanyID",
                table: "CompanyMovie");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMovie_Movies_MovieID",
                table: "CompanyMovie");

            migrationBuilder.DropForeignKey(
                name: "FK_GenreMovie_Genres_GenreID",
                table: "GenreMovie");

            migrationBuilder.DropForeignKey(
                name: "FK_GenreMovie_Movies_MovieID",
                table: "GenreMovie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GenreMovie",
                table: "GenreMovie");

            migrationBuilder.DropIndex(
                name: "IX_GenreMovie_GenreID",
                table: "GenreMovie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyMovie",
                table: "CompanyMovie");

            migrationBuilder.DropIndex(
                name: "IX_CompanyMovie_CompanyID",
                table: "CompanyMovie");

            migrationBuilder.DropColumn(
                name: "GenreMovieID",
                table: "GenreMovie");

            migrationBuilder.DropColumn(
                name: "CompanyMovieID",
                table: "CompanyMovie");

            migrationBuilder.RenameColumn(
                name: "MovieID",
                table: "GenreMovie",
                newName: "MoviesID");

            migrationBuilder.RenameColumn(
                name: "GenreID",
                table: "GenreMovie",
                newName: "GenresID");

            migrationBuilder.RenameIndex(
                name: "IX_GenreMovie_MovieID",
                table: "GenreMovie",
                newName: "IX_GenreMovie_MoviesID");

            migrationBuilder.RenameColumn(
                name: "MovieID",
                table: "CompanyMovie",
                newName: "MoviesID");

            migrationBuilder.RenameColumn(
                name: "CompanyID",
                table: "CompanyMovie",
                newName: "CompaniesID");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMovie_MovieID",
                table: "CompanyMovie",
                newName: "IX_CompanyMovie_MoviesID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GenreMovie",
                table: "GenreMovie",
                columns: new[] { "GenresID", "MoviesID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyMovie",
                table: "CompanyMovie",
                columns: new[] { "CompaniesID", "MoviesID" });

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMovie_Companies_CompaniesID",
                table: "CompanyMovie",
                column: "CompaniesID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMovie_Movies_MoviesID",
                table: "CompanyMovie",
                column: "MoviesID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GenreMovie_Genres_GenresID",
                table: "GenreMovie",
                column: "GenresID",
                principalTable: "Genres",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GenreMovie_Movies_MoviesID",
                table: "GenreMovie",
                column: "MoviesID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
