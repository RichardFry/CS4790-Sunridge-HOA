using Microsoft.EntityFrameworkCore.Migrations;

namespace SunridgeHOA.Migrations
{
    public partial class Updating_PhotoId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PhotoId",
                table: "BoardMember",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoardMember_PhotoId",
                table: "BoardMember",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardMember_Photo_PhotoId",
                table: "BoardMember",
                column: "PhotoId",
                principalTable: "Photo",
                principalColumn: "PhotoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardMember_Photo_PhotoId",
                table: "BoardMember");

            migrationBuilder.DropIndex(
                name: "IX_BoardMember_PhotoId",
                table: "BoardMember");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoId",
                table: "BoardMember",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
