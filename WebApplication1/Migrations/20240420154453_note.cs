using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class note : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "QR_id",
                table: "Table",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableId = table.Column<int>(type: "int", nullable: false),
                    FoodId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ManageStatusId = table.Column<int>(type: "int", nullable: false)
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
                        name: "FK_Cart_ManageStatus_ManageStatusId",
                        column: x => x.ManageStatusId,
                        principalTable: "ManageStatus",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_Cart_ManageStatusId",
                table: "Cart",
                column: "ManageStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_StatusId",
                table: "Cart",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_TableId",
                table: "Cart",
                column: "TableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.AlterColumn<string>(
                name: "QR_id",
                table: "Table",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
