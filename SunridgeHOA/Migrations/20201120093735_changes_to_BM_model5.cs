using Microsoft.EntityFrameworkCore.Migrations;

namespace SunridgeHOA.Migrations
{
    public partial class changes_to_BM_model5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "BoardMember");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoId",
                table: "BoardMember",
                nullable: true);
        }
    }
}
