using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class updatee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
