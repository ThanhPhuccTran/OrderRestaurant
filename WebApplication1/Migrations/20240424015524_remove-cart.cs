using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class removecart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.AlterColumn<decimal>(
                name: "Pay",
                table: "Order",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Pay",
                table: "Order",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    FoodId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    TableId = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_Cart_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_Cart_Food_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Food",
                        principalColumn: "FoodId");
                    table.ForeignKey(
                        name: "FK_Cart_ManageStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ManageStatus",
                        principalColumn: "StatusId");
                    table.ForeignKey(
                        name: "FK_Cart_Table_TableId",
                        column: x => x.TableId,
                        principalTable: "Table",
                        principalColumn: "TableId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cart_EmployeeId",
                table: "Cart",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_FoodId",
                table: "Cart",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_StatusId",
                table: "Cart",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_TableId",
                table: "Cart",
                column: "TableId");
        }
    }
}
