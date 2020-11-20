using Microsoft.EntityFrameworkCore.Migrations;

namespace SunridgeHOA.Migrations
{
    public partial class changes_to_BM_model3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardMember_OwnerLot_OwnerLotId",
                table: "BoardMember");

            migrationBuilder.DropIndex(
                name: "IX_BoardMember_OwnerLotId",
                table: "BoardMember");

            migrationBuilder.DropColumn(
                name: "OwnerLotId",
                table: "BoardMember");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerLotId",
                table: "BoardMember",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BoardMember_OwnerLotId",
                table: "BoardMember",
                column: "OwnerLotId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardMember_OwnerLot_OwnerLotId",
                table: "BoardMember",
                column: "OwnerLotId",
                principalTable: "OwnerLot",
                principalColumn: "OwnerLotId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
