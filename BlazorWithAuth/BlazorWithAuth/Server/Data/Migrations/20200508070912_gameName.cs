using Microsoft.EntityFrameworkCore.Migrations;

namespace CardGames.Server.Data.Migrations
{
    public partial class gameName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GameName",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameName",
                table: "AspNetUsers");
        }
    }
}
