using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matsedeln.Migrations
{
    /// <inheritdoc />
    public partial class FixRecipeIngredientRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecipeId1",
                table: "Ingredients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_RecipeId1",
                table: "Ingredients",
                column: "RecipeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recipes_RecipeId1",
                table: "Ingredients",
                column: "RecipeId1",
                principalTable: "Recipes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recipes_RecipeId1",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_RecipeId1",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "RecipeId1",
                table: "Ingredients");
        }
    }
}
