using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddJoinTableToDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                newName: "CrewMember");

            migrationBuilder.RenameIndex(
                name: "IX_CrewMembers_PersonID",
                table: "CrewMember",
                newName: "IX_CrewMember_PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_CrewMembers_MovieID",
                table: "CrewMember",
                newName: "IX_CrewMember_MovieID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CrewMember",
                table: "CrewMember",
                column: "CrewMemberID");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrewMember_Movies_MovieID",
                table: "CrewMember");

            migrationBuilder.DropForeignKey(
                name: "FK_CrewMember_Persons_PersonID",
                table: "CrewMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CrewMember",
                table: "CrewMember");

            migrationBuilder.RenameTable(
                name: "CrewMember",
                newName: "CrewMembers");

            migrationBuilder.RenameIndex(
                name: "IX_CrewMember_PersonID",
                table: "CrewMembers",
                newName: "IX_CrewMembers_PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_CrewMember_MovieID",
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
    }
}
