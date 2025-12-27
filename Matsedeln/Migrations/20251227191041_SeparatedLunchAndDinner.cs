using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matsedeln.Migrations
{
    /// <inheritdoc />
    public partial class SeparatedLunchAndDinner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_Recipes_RecipeId",
                table: "MenuItems");

            migrationBuilder.RenameColumn(
                name: "RecipeId",
                table: "MenuItems",
                newName: "LunchRecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItems_RecipeId",
                table: "MenuItems",
                newName: "IX_MenuItems_LunchRecipeId");

            migrationBuilder.AddColumn<int>(
                name: "DinnerRecipeId",
                table: "MenuItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_DinnerRecipeId",
                table: "MenuItems",
                column: "DinnerRecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Recipes_DinnerRecipeId",
                table: "MenuItems",
                column: "DinnerRecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Recipes_LunchRecipeId",
                table: "MenuItems",
                column: "LunchRecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_Recipes_DinnerRecipeId",
                table: "MenuItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_Recipes_LunchRecipeId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_DinnerRecipeId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "DinnerRecipeId",
                table: "MenuItems");

            migrationBuilder.RenameColumn(
                name: "LunchRecipeId",
                table: "MenuItems",
                newName: "RecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItems_LunchRecipeId",
                table: "MenuItems",
                newName: "IX_MenuItems_RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Recipes_RecipeId",
                table: "MenuItems",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
