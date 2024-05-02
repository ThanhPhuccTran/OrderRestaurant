using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class add_role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Employee",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RolesRoleId",
                table: "Employee",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Roles_RolesRoleId",
                table: "Employee");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Employee_RolesRoleId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "RolesRoleId",
                table: "Employee");
        }
    }
}
