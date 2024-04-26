using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class Update_Config : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Table",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Order",
                newName: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Table",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Order",
                newName: "StatusId");
        }
    }
}
