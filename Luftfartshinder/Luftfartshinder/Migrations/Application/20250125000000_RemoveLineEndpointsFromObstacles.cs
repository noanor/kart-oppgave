using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luftfartshinder.Migrations.Application
{
    /// <inheritdoc />
    public partial class RemoveLineEndpointsFromObstacles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndLatitude",
                table: "Obstacles");

            migrationBuilder.DropColumn(
                name: "EndLongitude",
                table: "Obstacles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}

