using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddMoviePersonToCrewRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrewRoles_Movies_MovieID",
                table: "CrewRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_CrewRoles_Persons_PersonID",
                table: "CrewRoles");

            migrationBuilder.AlterColumn<int>(
                name: "PersonID",
                table: "CrewRoles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MovieID",
                table: "CrewRoles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CrewRoles_Movies_MovieID",
                table: "CrewRoles",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CrewRoles_Persons_PersonID",
                table: "CrewRoles",
                column: "PersonID",
                principalTable: "Persons",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrewRoles_Movies_MovieID",
                table: "CrewRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_CrewRoles_Persons_PersonID",
                table: "CrewRoles");

            migrationBuilder.AlterColumn<int>(
                name: "PersonID",
                table: "CrewRoles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MovieID",
                table: "CrewRoles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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
    }
}
