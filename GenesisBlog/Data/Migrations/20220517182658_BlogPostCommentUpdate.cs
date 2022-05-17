using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenesisBlog.Data.Migrations
{
    public partial class BlogPostCommentUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModeratorFullName",
                table: "BlogPostComment");

            migrationBuilder.AddColumn<string>(
                name: "ModeratorName",
                table: "BlogPostComment",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModeratorName",
                table: "BlogPostComment");

            migrationBuilder.AddColumn<string>(
                name: "ModeratorFullName",
                table: "BlogPostComment",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
