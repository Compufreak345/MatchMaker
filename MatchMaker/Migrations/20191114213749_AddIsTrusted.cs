using Microsoft.EntityFrameworkCore.Migrations;

namespace MatchMaker.Migrations
{
    public partial class AddIsTrusted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTrusted",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTrusted",
                table: "AspNetUsers");
        }
    }
}
