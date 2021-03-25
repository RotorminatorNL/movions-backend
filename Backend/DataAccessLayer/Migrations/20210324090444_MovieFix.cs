using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class MovieFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Companies_CompanyID",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Genres_GenreID",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Languages_LanguageID",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_CompanyID",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_GenreID",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_LanguageID",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "GenreID",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "Movies");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                table: "Movies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GenreID",
                table: "Movies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LanguageID",
                table: "Movies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CompanyID",
                table: "Movies",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_GenreID",
                table: "Movies",
                column: "GenreID");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_LanguageID",
                table: "Movies",
                column: "LanguageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Companies_CompanyID",
                table: "Movies",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Genres_GenreID",
                table: "Movies",
                column: "GenreID",
                principalTable: "Genres",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Languages_LanguageID",
                table: "Movies",
                column: "LanguageID",
                principalTable: "Languages",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
