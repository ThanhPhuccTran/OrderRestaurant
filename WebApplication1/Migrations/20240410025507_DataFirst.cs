using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class DataFirst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    idCategory = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.idCategory);
                });

            migrationBuilder.CreateTable(
                name: "Food",
                columns: table => new
                {
                    idFood = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameFood = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitPrice = table.Column<double>(type: "float", nullable: false),
                    UrlImage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    idCategory = table.Column<int>(type: "int", nullable: false),
                    CategoryFoodsidCategory = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Food", x => x.idFood);
                    table.ForeignKey(
                        name: "FK_Food_Category_CategoryFoodsidCategory",
                        column: x => x.CategoryFoodsidCategory,
                        principalTable: "Category",
                        principalColumn: "idCategory");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Food_CategoryFoodsidCategory",
                table: "Food",
                column: "CategoryFoodsidCategory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Food");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
