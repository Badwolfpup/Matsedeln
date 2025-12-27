using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matsedeln.Migrations
{
    /// <inheritdoc />
    public partial class AddeCascadeNoAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_Recipes_DinnerRecipeId",
                table: "MenuItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_Recipes_LunchRecipeId",
                table: "MenuItems");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Recipes_DinnerRecipeId",
                table: "MenuItems",
                column: "DinnerRecipeId",
                principalTable: "Recipes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Recipes_LunchRecipeId",
                table: "MenuItems",
                column: "LunchRecipeId",
                principalTable: "Recipes",
                principalColumn: "Id");
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

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Recipes_DinnerRecipeId",
                table: "MenuItems",
                column: "DinnerRecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Recipes_LunchRecipeId",
                table: "MenuItems",
                column: "LunchRecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
