using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matserver.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GramsPerDeciliter = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    GramsPerStick = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsInRecipe = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "g"),
                    Quantity = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    QuantityInGram = table.Column<int>(type: "int", nullable: false),
                    QuantityInDl = table.Column<int>(type: "int", nullable: false),
                    QuantityInSt = table.Column<int>(type: "int", nullable: false),
                    QuantityInMsk = table.Column<int>(type: "int", nullable: false),
                    QuantityInTsk = table.Column<int>(type: "int", nullable: false),
                    QuantityInKrm = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredients_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ingredients_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LunchRecipeId = table.Column<int>(type: "int", nullable: true),
                    DinnerRecipeId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuEntries_Recipes_DinnerRecipeId",
                        column: x => x.DinnerRecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MenuEntries_Recipes_LunchRecipeId",
                        column: x => x.LunchRecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecipeHierarchy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentRecipeId = table.Column<int>(type: "int", nullable: false),
                    ChildRecipeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeHierarchy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeHierarchy_Recipes_ChildRecipeId",
                        column: x => x.ChildRecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecipeHierarchy_Recipes_ParentRecipeId",
                        column: x => x.ParentRecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_GoodsId",
                table: "Ingredients",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_RecipeId",
                table: "Ingredients",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuEntries_DinnerRecipeId",
                table: "MenuEntries",
                column: "DinnerRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuEntries_LunchRecipeId",
                table: "MenuEntries",
                column: "LunchRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeHierarchy_ChildRecipeId",
                table: "RecipeHierarchy",
                column: "ChildRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeHierarchy_ParentRecipeId",
                table: "RecipeHierarchy",
                column: "ParentRecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "MenuEntries");

            migrationBuilder.DropTable(
                name: "RecipeHierarchy");

            migrationBuilder.DropTable(
                name: "Goods");

            migrationBuilder.DropTable(
                name: "Recipes");
        }
    }
}
