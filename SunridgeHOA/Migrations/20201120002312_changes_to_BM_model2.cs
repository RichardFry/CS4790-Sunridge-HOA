using Microsoft.EntityFrameworkCore.Migrations;

namespace SunridgeHOA.Migrations
{
    public partial class changes_to_BM_model2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardMember_Photo_PhotoId1",
                table: "BoardMember");

            migrationBuilder.DropIndex(
                name: "IX_BoardMember_PhotoId1",
                table: "BoardMember");

            migrationBuilder.DropColumn(
                name: "PhotoId1",
                table: "BoardMember");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhotoId1",
                table: "BoardMember",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoardMember_PhotoId1",
                table: "BoardMember",
                column: "PhotoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardMember_Photo_PhotoId1",
                table: "BoardMember",
                column: "PhotoId1",
                principalTable: "Photo",
                principalColumn: "PhotoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
