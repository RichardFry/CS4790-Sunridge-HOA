using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SunridgeHOA.Migrations
{
    public partial class chaningBoardMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardMember",
                columns: table => new
                {
                    BoardMemberId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BoardPosition = table.Column<string>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    LotId = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    test = table.Column<int>(nullable: false),
                    PhotoId = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    PhotoIdFK = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardMember", x => x.BoardMemberId);
                    table.ForeignKey(
                        name: "FK_BoardMember_Lot_LotId",
                        column: x => x.LotId,
                        principalTable: "Lot",
                        principalColumn: "LotId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoardMember_Owner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owner",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_BoardMember_Photo_PhotoIdFK",
                        column: x => x.PhotoIdFK,
                        principalTable: "Photo",
                        principalColumn: "PhotoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardMember_LotId",
                table: "BoardMember",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardMember_OwnerId",
                table: "BoardMember",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardMember_PhotoIdFK",
                table: "BoardMember",
                column: "PhotoIdFK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardMember");
        }
    }
}
