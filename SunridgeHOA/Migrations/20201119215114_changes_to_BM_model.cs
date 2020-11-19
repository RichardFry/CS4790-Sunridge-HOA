using Microsoft.EntityFrameworkCore.Migrations;

namespace SunridgeHOA.Migrations
{
    public partial class changes_to_BM_model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardMember_Lot_LotId",
                table: "BoardMember");

            migrationBuilder.DropForeignKey(
                name: "FK_BoardMember_Photo_PhotoIdFK",
                table: "BoardMember");

            migrationBuilder.DropIndex(
                name: "IX_BoardMember_LotId",
                table: "BoardMember");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "BoardMember");

            migrationBuilder.RenameColumn(
                name: "test",
                table: "BoardMember",
                newName: "OwnerLotId");

            migrationBuilder.RenameColumn(
                name: "PhotoIdFK",
                table: "BoardMember",
                newName: "PhotoId1");

            migrationBuilder.RenameIndex(
                name: "IX_BoardMember_PhotoIdFK",
                table: "BoardMember",
                newName: "IX_BoardMember_PhotoId1");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoId",
                table: "BoardMember",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "BoardPosition",
                table: "BoardMember",
                nullable: true,
                oldClrType: typeof(string));

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

            migrationBuilder.AddForeignKey(
                name: "FK_BoardMember_Photo_PhotoId1",
                table: "BoardMember",
                column: "PhotoId1",
                principalTable: "Photo",
                principalColumn: "PhotoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardMember_OwnerLot_OwnerLotId",
                table: "BoardMember");

            migrationBuilder.DropForeignKey(
                name: "FK_BoardMember_Photo_PhotoId1",
                table: "BoardMember");

            migrationBuilder.DropIndex(
                name: "IX_BoardMember_OwnerLotId",
                table: "BoardMember");

            migrationBuilder.RenameColumn(
                name: "PhotoId1",
                table: "BoardMember",
                newName: "PhotoIdFK");

            migrationBuilder.RenameColumn(
                name: "OwnerLotId",
                table: "BoardMember",
                newName: "test");

            migrationBuilder.RenameIndex(
                name: "IX_BoardMember_PhotoId1",
                table: "BoardMember",
                newName: "IX_BoardMember_PhotoIdFK");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoId",
                table: "BoardMember",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BoardPosition",
                table: "BoardMember",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LotId",
                table: "BoardMember",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BoardMember_LotId",
                table: "BoardMember",
                column: "LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardMember_Lot_LotId",
                table: "BoardMember",
                column: "LotId",
                principalTable: "Lot",
                principalColumn: "LotId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BoardMember_Photo_PhotoIdFK",
                table: "BoardMember",
                column: "PhotoIdFK",
                principalTable: "Photo",
                principalColumn: "PhotoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
