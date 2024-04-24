using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class update_settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_ManageStatus_StatusId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Table_ManageStatus_StatusId",
                table: "Table");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ManageStatus",
                table: "ManageStatus");

            migrationBuilder.RenameTable(
                name: "ManageStatus",
                newName: "Settings");

            migrationBuilder.AddColumn<int>(
                name: "Code",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Settings",
                table: "Settings",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Settings_StatusId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Table_Settings_StatusId",
                table: "Table");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Settings",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Settings");

            migrationBuilder.RenameTable(
                name: "Settings",
                newName: "ManageStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ManageStatus",
                table: "ManageStatus",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_ManageStatus_StatusId",
                table: "Order",
                column: "StatusId",
                principalTable: "ManageStatus",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Table_ManageStatus_StatusId",
                table: "Table",
                column: "StatusId",
                principalTable: "ManageStatus",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
