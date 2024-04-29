using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class Update_new_Requirement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Table_Requirements_RequirementsRequestId",
                table: "Table");

            migrationBuilder.DropIndex(
                name: "IX_Table_RequirementsRequestId",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "RequirementsRequestId",
                table: "Table");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_TableId",
                table: "Requirements",
                column: "TableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_Table_TableId",
                table: "Requirements",
                column: "TableId",
                principalTable: "Table",
                principalColumn: "TableId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_Table_TableId",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_TableId",
                table: "Requirements");

            migrationBuilder.AddColumn<int>(
                name: "RequirementsRequestId",
                table: "Table",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Table_RequirementsRequestId",
                table: "Table",
                column: "RequirementsRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Table_Requirements_RequirementsRequestId",
                table: "Table",
                column: "RequirementsRequestId",
                principalTable: "Requirements",
                principalColumn: "RequestId");
        }
    }
}
