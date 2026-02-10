using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matserver.Migrations
{
    /// <inheritdoc />
    public partial class IsDish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDish",
                table: "Recipes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDish",
                table: "Recipes");
        }
    }
}
