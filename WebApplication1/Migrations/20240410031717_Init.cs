using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Food_Category_CategoryFoodsidCategory",
                table: "Food");

            migrationBuilder.DropIndex(
                name: "IX_Food_CategoryFoodsidCategory",
                table: "Food");

            migrationBuilder.DropColumn(
                name: "CategoryFoodsidCategory",
                table: "Food");

            migrationBuilder.RenameColumn(
                name: "idCategory",
                table: "Food",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "idFood",
                table: "Food",
                newName: "FoodId");

            migrationBuilder.RenameColumn(
                name: "idCategory",
                table: "Category",
                newName: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Food_CategoryId",
                table: "Food",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Food_Category_CategoryId",
                table: "Food",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Food_Category_CategoryId",
                table: "Food");

            migrationBuilder.DropIndex(
                name: "IX_Food_CategoryId",
                table: "Food");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Food",
                newName: "idCategory");

            migrationBuilder.RenameColumn(
                name: "FoodId",
                table: "Food",
                newName: "idFood");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Category",
                newName: "idCategory");

            migrationBuilder.AddColumn<int>(
                name: "CategoryFoodsidCategory",
                table: "Food",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Food_CategoryFoodsidCategory",
                table: "Food",
                column: "CategoryFoodsidCategory");

            migrationBuilder.AddForeignKey(
                name: "FK_Food_Category_CategoryFoodsidCategory",
                table: "Food",
                column: "CategoryFoodsidCategory",
                principalTable: "Category",
                principalColumn: "idCategory");
        }
    }
}
