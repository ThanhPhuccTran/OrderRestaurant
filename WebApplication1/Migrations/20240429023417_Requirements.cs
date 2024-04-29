using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class Requirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequirementsRequestId",
                table: "Table",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TablesTableId",
                table: "Table",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Requirements",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableId = table.Column<int>(type: "int", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestNode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirements", x => x.RequestId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Table_RequirementsRequestId",
                table: "Table",
                column: "RequirementsRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Table_TablesTableId",
                table: "Table",
                column: "TablesTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Table_Requirements_RequirementsRequestId",
                table: "Table",
                column: "RequirementsRequestId",
                principalTable: "Requirements",
                principalColumn: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Table_Table_TablesTableId",
                table: "Table",
                column: "TablesTableId",
                principalTable: "Table",
                principalColumn: "TableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Table_Requirements_RequirementsRequestId",
                table: "Table");

            migrationBuilder.DropForeignKey(
                name: "FK_Table_Table_TablesTableId",
                table: "Table");

            migrationBuilder.DropTable(
                name: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_Table_RequirementsRequestId",
                table: "Table");

            migrationBuilder.DropIndex(
                name: "IX_Table_TablesTableId",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "RequirementsRequestId",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "TablesTableId",
                table: "Table");
        }
    }
}
