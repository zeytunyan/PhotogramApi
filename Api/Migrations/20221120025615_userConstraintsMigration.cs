using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    public partial class userConstraintsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "\"Users\".\"Information_Gender\"",
                table: "Users",
                sql: "\"Users\".\"Information_Gender\" IN ('Man', 'Woman', 'Another')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "\"Users\".\"Information_Gender\"",
                table: "Users");
        }
    }
}
