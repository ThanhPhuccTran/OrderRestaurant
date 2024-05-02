using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class update_roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Roles_RolesRoleId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_RolesRoleId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "RolesRoleId",
                table: "Employee");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_RoleId",
                table: "Employee",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Roles_RoleId",
                table: "Employee",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Roles_RoleId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_RoleId",
                table: "Employee");

            migrationBuilder.AddColumn<int>(
                name: "RolesRoleId",
                table: "Employee",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_RolesRoleId",
                table: "Employee",
                column: "RolesRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Roles_RolesRoleId",
                table: "Employee",
                column: "RolesRoleId",
                principalTable: "Roles",
                principalColumn: "RoleId");
        }
    }
}
