using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class Update_Permissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Permission",
                table: "Permission");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_Permission",
                table: "Permission",
                column: "RoleName");
        }
    }
}
