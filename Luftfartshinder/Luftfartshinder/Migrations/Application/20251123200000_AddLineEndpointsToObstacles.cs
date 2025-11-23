using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luftfartshinder.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddLineEndpointsToObstacles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EndLatitude",
                table: "Obstacles",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EndLongitude",
                table: "Obstacles",
                type: "double",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndLatitude",
                table: "Obstacles");

            migrationBuilder.DropColumn(
                name: "EndLongitude",
                table: "Obstacles");
        }
    }
}

