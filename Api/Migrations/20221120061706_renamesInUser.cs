using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    public partial class renamesInUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "\"Users\".\"Information_Gender\"",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Profile_Surname",
                table: "Users",
                newName: "Surname");

            migrationBuilder.RenameColumn(
                name: "Profile_Status",
                table: "Users",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Profile_Profession",
                table: "Users",
                newName: "Profession");

            migrationBuilder.RenameColumn(
                name: "Profile_PasswordHash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "Profile_GivenName",
                table: "Users",
                newName: "GivenName");

            migrationBuilder.RenameColumn(
                name: "Profile_Gender",
                table: "Users",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "Profile_FullName",
                table: "Users",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "Profile_Country",
                table: "Users",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "Profile_BirthDate",
                table: "Users",
                newName: "BirthDate");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "text",
                nullable: false,
                computedColumnSql: "\"Users\".\"GivenName\" || ' ' || \"Users\".\"Surname\"",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldComputedColumnSql: "\"Users\".\"Information_GivenName\" || ' ' || \"Users\".\"Information_Surname\"",
                oldStored: true);

            migrationBuilder.AddCheckConstraint(
                name: "\"Users\".\"Gender\"",
                table: "Users",
                sql: "\"Users\".\"Gender\" IN ('Man', 'Woman', 'Another')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "\"Users\".\"Gender\"",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Users",
                newName: "Profile_Surname");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Users",
                newName: "Profile_Status");

            migrationBuilder.RenameColumn(
                name: "Profession",
                table: "Users",
                newName: "Profile_Profession");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Profile_PasswordHash");

            migrationBuilder.RenameColumn(
                name: "GivenName",
                table: "Users",
                newName: "Profile_GivenName");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Users",
                newName: "Profile_Gender");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Users",
                newName: "Profile_FullName");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "Users",
                newName: "Profile_Country");

            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "Users",
                newName: "Profile_BirthDate");

            migrationBuilder.AlterColumn<string>(
                name: "Profile_FullName",
                table: "Users",
                type: "text",
                nullable: false,
                computedColumnSql: "\"Users\".\"Information_GivenName\" || ' ' || \"Users\".\"Information_Surname\"",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldComputedColumnSql: "\"Users\".\"GivenName\" || ' ' || \"Users\".\"Surname\"",
                oldStored: true);

            migrationBuilder.AddCheckConstraint(
                name: "\"Users\".\"Information_Gender\"",
                table: "Users",
                sql: "\"Users\".\"Information_Gender\" IN ('Man', 'Woman', 'Another')");
        }
    }
}
