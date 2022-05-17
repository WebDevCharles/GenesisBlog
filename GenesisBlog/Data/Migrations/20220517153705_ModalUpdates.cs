using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenesisBlog.Data.Migrations
{
    public partial class ModalUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModeratedId",
                table: "BlogPostComment",
                newName: "ModeratorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModeratorId",
                table: "BlogPostComment",
                newName: "ModeratedId");
        }
    }
}
