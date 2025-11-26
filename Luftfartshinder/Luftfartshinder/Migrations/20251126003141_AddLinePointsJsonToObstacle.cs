using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luftfartshinder.Migrations
{
    /// <inheritdoc />
    public partial class AddLinePointsJsonToObstacle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinePointsJson",
                table: "Obstacles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinePointsJson",
                table: "Obstacles");
        }
    }
}
