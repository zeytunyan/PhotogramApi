using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    public partial class likesPkChangedAndConstraintChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "\"Users\".\"Gender\"",
                table: "Users");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PostLikes_UserId_PostId_Date",
                table: "PostLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostLikes",
                table: "PostLikes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CommentLikes_UserId_CommentId_Date",
                table: "CommentLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentLikes",
                table: "CommentLikes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PostLikes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CommentLikes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostLikes",
                table: "PostLikes",
                columns: new[] { "UserId", "PostId", "Date" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentLikes",
                table: "CommentLikes",
                columns: new[] { "UserId", "CommentId", "Date" });

            migrationBuilder.AddCheckConstraint(
                name: "\"Users\".\"Gender\"",
                table: "Users",
                sql: "\"Users\".\"Gender\" IN ('Man', 'Woman', 'Another', 'man', 'woman', 'another')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "\"Users\".\"Gender\"",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostLikes",
                table: "PostLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentLikes",
                table: "CommentLikes");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PostLikes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CommentLikes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_PostLikes_UserId_PostId_Date",
                table: "PostLikes",
                columns: new[] { "UserId", "PostId", "Date" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostLikes",
                table: "PostLikes",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CommentLikes_UserId_CommentId_Date",
                table: "CommentLikes",
                columns: new[] { "UserId", "CommentId", "Date" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentLikes",
                table: "CommentLikes",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "\"Users\".\"Gender\"",
                table: "Users",
                sql: "\"Users\".\"Gender\" IN ('Man', 'Woman', 'Another')");
        }
    }
}
