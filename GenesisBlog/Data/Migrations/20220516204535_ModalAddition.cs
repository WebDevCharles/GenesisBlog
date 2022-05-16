using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenesisBlog.Data.Migrations
{
    public partial class ModalAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "BlogPostComment",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Updated",
                table: "BlogPostComment");
        }
    }
}
