using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Table_Table_TablesTableId",
                table: "Table");

            migrationBuilder.DropIndex(
                name: "IX_Table_TablesTableId",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "TablesTableId",
                table: "Table");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TablesTableId",
                table: "Table",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Table_TablesTableId",
                table: "Table",
                column: "TablesTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Table_Table_TablesTableId",
                table: "Table",
                column: "TablesTableId",
                principalTable: "Table",
                principalColumn: "TableId");
        }
    }
}
