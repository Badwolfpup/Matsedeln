using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matsedeln.Migrations
{
    /// <inheritdoc />
    public partial class AddedChosenUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChosenUnit",
                table: "Ingredients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "g");

            migrationBuilder.Sql("UPDATE Ingredients SET ChosenUnit = 'g' WHERE ChosenUnit IS NULL;");  // NEW: Update existing rows

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChosenUnit",
                table: "Ingredients");
        }
    }
}
