using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    public partial class renameName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_Name",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "NickName");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_NickName",
                table: "Users",
                column: "NickName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_NickName",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "NickName",
                table: "Users",
                newName: "Name");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_Name",
                table: "Users",
                column: "Name");
        }
    }
}
