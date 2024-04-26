using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class Config : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Settings_StatusId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Table_Settings_StatusId",
                table: "Table");

            migrationBuilder.DropIndex(
                name: "IX_Table_StatusId",
                table: "Table");

            migrationBuilder.DropIndex(
                name: "IX_Order_StatusId",
                table: "Order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Table_StatusId",
                table: "Table",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_StatusId",
                table: "Order",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Settings_StatusId",
                table: "Order",
                column: "StatusId",
                principalTable: "Settings",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Table_Settings_StatusId",
                table: "Table",
                column: "StatusId",
                principalTable: "Settings",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
