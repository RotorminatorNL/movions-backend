using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class CrewRole2CrewMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CrewRoleID",
                table: "CrewRoles",
                newName: "CrewMemberID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CrewMemberID",
                table: "CrewRoles",
                newName: "CrewRoleID");
        }
    }
}
