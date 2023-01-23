using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    public partial class followings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Settings_IsPrivate",
                table: "Users",
                newName: "IsPrivate");

            migrationBuilder.RenameColumn(
                name: "Settings_IsDeleted",
                table: "Users",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Information_Surname",
                table: "Users",
                newName: "Profile_Surname");

            migrationBuilder.RenameColumn(
                name: "Information_Status",
                table: "Users",
                newName: "Profile_Status");

            migrationBuilder.RenameColumn(
                name: "Information_Profession",
                table: "Users",
                newName: "Profile_Profession");

            migrationBuilder.RenameColumn(
                name: "Information_PasswordHash",
                table: "Users",
                newName: "Profile_PasswordHash");

            migrationBuilder.RenameColumn(
                name: "Information_GivenName",
                table: "Users",
                newName: "Profile_GivenName");

            migrationBuilder.RenameColumn(
                name: "Information_Gender",
                table: "Users",
                newName: "Profile_Gender");

            migrationBuilder.RenameColumn(
                name: "Information_FullName",
                table: "Users",
                newName: "Profile_FullName");

            migrationBuilder.RenameColumn(
                name: "Information_Country",
                table: "Users",
                newName: "Profile_Country");

            migrationBuilder.RenameColumn(
                name: "Information_BirthDate",
                table: "Users",
                newName: "Profile_BirthDate");

            migrationBuilder.CreateTable(
                name: "Followings",
                columns: table => new
                {
                    FollowerId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowedToId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UnfollowDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Followings", x => new { x.FollowerId, x.FollowedToId, x.FollowDate });
                    table.ForeignKey(
                        name: "FK_Followings_Users_FollowedToId",
                        column: x => x.FollowedToId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Followings_Users_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserUser",
                columns: table => new
                {
                    FollowersId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowingsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUser", x => new { x.FollowersId, x.FollowingsId });
                    table.ForeignKey(
                        name: "FK_UserUser_Users_FollowersId",
                        column: x => x.FollowersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserUser_Users_FollowingsId",
                        column: x => x.FollowingsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Followings_FollowedToId",
                table: "Followings",
                column: "FollowedToId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUser_FollowingsId",
                table: "UserUser",
                column: "FollowingsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Followings");

            migrationBuilder.DropTable(
                name: "UserUser");

            migrationBuilder.RenameColumn(
                name: "Profile_Surname",
                table: "Users",
                newName: "Information_Surname");

            migrationBuilder.RenameColumn(
                name: "Profile_Status",
                table: "Users",
                newName: "Information_Status");

            migrationBuilder.RenameColumn(
                name: "Profile_Profession",
                table: "Users",
                newName: "Information_Profession");

            migrationBuilder.RenameColumn(
                name: "Profile_PasswordHash",
                table: "Users",
                newName: "Information_PasswordHash");

            migrationBuilder.RenameColumn(
                name: "Profile_GivenName",
                table: "Users",
                newName: "Information_GivenName");

            migrationBuilder.RenameColumn(
                name: "Profile_Gender",
                table: "Users",
                newName: "Information_Gender");

            migrationBuilder.RenameColumn(
                name: "Profile_FullName",
                table: "Users",
                newName: "Information_FullName");

            migrationBuilder.RenameColumn(
                name: "Profile_Country",
                table: "Users",
                newName: "Information_Country");

            migrationBuilder.RenameColumn(
                name: "Profile_BirthDate",
                table: "Users",
                newName: "Information_BirthDate");

            migrationBuilder.RenameColumn(
                name: "IsPrivate",
                table: "Users",
                newName: "Settings_IsPrivate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Users",
                newName: "Settings_IsDeleted");
        }
    }
}
