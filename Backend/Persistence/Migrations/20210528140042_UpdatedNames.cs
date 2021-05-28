using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class UpdatedNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMovie_Companies_CompanyID",
                table: "CompanyMovie");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMovie_Movies_MovieID",
                table: "CompanyMovie");

            migrationBuilder.DropForeignKey(
                name: "FK_CrewMember_Movies_MovieID",
                table: "CrewMember");

            migrationBuilder.DropForeignKey(
                name: "FK_CrewMember_Persons_PersonID",
                table: "CrewMember");

            migrationBuilder.DropForeignKey(
                name: "FK_GenreMovie_Genres_GenreID",
                table: "GenreMovie");

            migrationBuilder.DropForeignKey(
                name: "FK_GenreMovie_Movies_MovieID",
                table: "GenreMovie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GenreMovie",
                table: "GenreMovie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CrewMember",
                table: "CrewMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyMovie",
                table: "CompanyMovie");

            migrationBuilder.RenameTable(
                name: "GenreMovie",
                newName: "GenreMovies");

            migrationBuilder.RenameTable(
                name: "CrewMember",
                newName: "CrewMembers");

            migrationBuilder.RenameTable(
                name: "CompanyMovie",
                newName: "CompanyMovies");

            migrationBuilder.RenameIndex(
                name: "IX_GenreMovie_MovieID",
                table: "GenreMovies",
                newName: "IX_GenreMovies_MovieID");

            migrationBuilder.RenameIndex(
                name: "IX_GenreMovie_GenreID",
                table: "GenreMovies",
                newName: "IX_GenreMovies_GenreID");

            migrationBuilder.RenameIndex(
                name: "IX_CrewMember_PersonID",
                table: "CrewMembers",
                newName: "IX_CrewMembers_PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_CrewMember_MovieID",
                table: "CrewMembers",
                newName: "IX_CrewMembers_MovieID");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMovie_MovieID",
                table: "CompanyMovies",
                newName: "IX_CompanyMovies_MovieID");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMovie_CompanyID",
                table: "CompanyMovies",
                newName: "IX_CompanyMovies_CompanyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GenreMovies",
                table: "GenreMovies",
                column: "GenreMovieID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CrewMembers",
                table: "CrewMembers",
                column: "CrewMemberID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyMovies",
                table: "CompanyMovies",
                column: "CompanyMovieID");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMovies_Companies_CompanyID",
                table: "CompanyMovies",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMovies_Movies_MovieID",
                table: "CompanyMovies",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CrewMembers_Movies_MovieID",
                table: "CrewMembers",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CrewMembers_Persons_PersonID",
                table: "CrewMembers",
                column: "PersonID",
                principalTable: "Persons",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GenreMovies_Genres_GenreID",
                table: "GenreMovies",
                column: "GenreID",
                principalTable: "Genres",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GenreMovies_Movies_MovieID",
                table: "GenreMovies",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMovies_Companies_CompanyID",
                table: "CompanyMovies");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMovies_Movies_MovieID",
                table: "CompanyMovies");

            migrationBuilder.DropForeignKey(
                name: "FK_CrewMembers_Movies_MovieID",
                table: "CrewMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_CrewMembers_Persons_PersonID",
                table: "CrewMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_GenreMovies_Genres_GenreID",
                table: "GenreMovies");

            migrationBuilder.DropForeignKey(
                name: "FK_GenreMovies_Movies_MovieID",
                table: "GenreMovies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GenreMovies",
                table: "GenreMovies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CrewMembers",
                table: "CrewMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyMovies",
                table: "CompanyMovies");

            migrationBuilder.RenameTable(
                name: "GenreMovies",
                newName: "GenreMovie");

            migrationBuilder.RenameTable(
                name: "CrewMembers",
                newName: "CrewMember");

            migrationBuilder.RenameTable(
                name: "CompanyMovies",
                newName: "CompanyMovie");

            migrationBuilder.RenameIndex(
                name: "IX_GenreMovies_MovieID",
                table: "GenreMovie",
                newName: "IX_GenreMovie_MovieID");

            migrationBuilder.RenameIndex(
                name: "IX_GenreMovies_GenreID",
                table: "GenreMovie",
                newName: "IX_GenreMovie_GenreID");

            migrationBuilder.RenameIndex(
                name: "IX_CrewMembers_PersonID",
                table: "CrewMember",
                newName: "IX_CrewMember_PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_CrewMembers_MovieID",
                table: "CrewMember",
                newName: "IX_CrewMember_MovieID");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMovies_MovieID",
                table: "CompanyMovie",
                newName: "IX_CompanyMovie_MovieID");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMovies_CompanyID",
                table: "CompanyMovie",
                newName: "IX_CompanyMovie_CompanyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GenreMovie",
                table: "GenreMovie",
                column: "GenreMovieID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CrewMember",
                table: "CrewMember",
                column: "CrewMemberID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyMovie",
                table: "CompanyMovie",
                column: "CompanyMovieID");

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
                name: "FK_CrewMember_Movies_MovieID",
                table: "CrewMember",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CrewMember_Persons_PersonID",
                table: "CrewMember",
                column: "PersonID",
                principalTable: "Persons",
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
    }
}
