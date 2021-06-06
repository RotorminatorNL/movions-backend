using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class TableCrewRolesChangeToCrewMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrewRoles_Movies_MovieID",
                table: "CrewRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_CrewRoles_Persons_PersonID",
                table: "CrewRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CrewRoles",
                table: "CrewRoles");

            migrationBuilder.RenameTable(
                name: "CrewRoles",
                newName: "CrewMembers");

            migrationBuilder.RenameIndex(
                name: "IX_CrewRoles_PersonID",
                table: "CrewMembers",
                newName: "IX_CrewMembers_PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_CrewRoles_MovieID",
                table: "CrewMembers",
                newName: "IX_CrewMembers_MovieID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CrewMembers",
                table: "CrewMembers",
                column: "CrewMemberID");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrewMembers_Movies_MovieID",
                table: "CrewMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_CrewMembers_Persons_PersonID",
                table: "CrewMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CrewMembers",
                table: "CrewMembers");

            migrationBuilder.RenameTable(
                name: "CrewMembers",
                newName: "CrewRoles");

            migrationBuilder.RenameIndex(
                name: "IX_CrewMembers_PersonID",
                table: "CrewRoles",
                newName: "IX_CrewRoles_PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_CrewMembers_MovieID",
                table: "CrewRoles",
                newName: "IX_CrewRoles_MovieID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CrewRoles",
                table: "CrewRoles",
                column: "CrewMemberID");

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
    }
}
