using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class updatecart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_ManageStatus_ManageStatusId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_ManageStatusId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "ManageStatusId",
                table: "Cart");

            migrationBuilder.AddColumn<int>(
                name: "ManageStatusStatusId",
                table: "Cart",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_ManageStatusStatusId",
                table: "Cart",
                column: "ManageStatusStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_ManageStatus_ManageStatusStatusId",
                table: "Cart",
                column: "ManageStatusStatusId",
                principalTable: "ManageStatus",
                principalColumn: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_ManageStatus_ManageStatusStatusId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_ManageStatusStatusId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "ManageStatusStatusId",
                table: "Cart");

            migrationBuilder.AddColumn<int>(
                name: "ManageStatusId",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_ManageStatusId",
                table: "Cart",
                column: "ManageStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_ManageStatus_ManageStatusId",
                table: "Cart",
                column: "ManageStatusId",
                principalTable: "ManageStatus",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
