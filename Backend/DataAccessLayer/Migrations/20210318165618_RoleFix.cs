using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class RoleFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonDTOID",
                table: "CrewRoles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CrewRoles_PersonDTOID",
                table: "CrewRoles",
                column: "PersonDTOID");

            migrationBuilder.AddForeignKey(
                name: "FK_CrewRoles_Persons_PersonDTOID",
                table: "CrewRoles",
                column: "PersonDTOID",
                principalTable: "Persons",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrewRoles_Persons_PersonDTOID",
                table: "CrewRoles");

            migrationBuilder.DropIndex(
                name: "IX_CrewRoles_PersonDTOID",
                table: "CrewRoles");

            migrationBuilder.DropColumn(
                name: "PersonDTOID",
                table: "CrewRoles");
        }
    }
}
