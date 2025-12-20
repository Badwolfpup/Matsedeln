using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matsedeln.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Imagepath",
                table: "Recipes",
                newName: "ImagePath");

            migrationBuilder.RenameColumn(
                name: "Imagepath",
                table: "Goods",
                newName: "ImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Recipes",
                newName: "Imagepath");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Goods",
                newName: "Imagepath");
        }
    }
}
