using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matsedeln.Migrations
{
    /// <inheritdoc />
    public partial class AddedRecipeHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old FK and column (from one-to-many)
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Recipes_ParentRecipeId",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_ParentRecipeId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "ParentRecipeId",
                table: "Recipes");

            // Create the new junction table
            migrationBuilder.CreateTable(
                name: "RecipeHierarchies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentRecipeId = table.Column<int>(type: "int", nullable: false),
                    ChildRecipeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeHierarchies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeHierarchies_Recipes_ChildRecipeId",
                        column: x => x.ChildRecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_RecipeHierarchies_Recipes_ParentRecipeId",
                        column: x => x.ParentRecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeHierarchies_ChildRecipeId",
                table: "RecipeHierarchies",
                column: "ChildRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeHierarchies_ParentRecipeId",
                table: "RecipeHierarchies",
                column: "ParentRecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeHierarchies");

            migrationBuilder.AddColumn<int>(
                name: "ParentRecipeId",
                table: "Recipes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_ParentRecipeId",
                table: "Recipes",
                column: "ParentRecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Recipes_ParentRecipeId",
                table: "Recipes",
                column: "ParentRecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}