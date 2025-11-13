using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luftfartshinder.Migrations
{
    /// <inheritdoc />
    public partial class migration1211 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObstacleHeight",
                table: "Obstacles");

            migrationBuilder.RenameColumn(
                name: "ObstacleName",
                table: "Obstacles",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "ObstacleDescription",
                table: "Obstacles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ObstacleCoords",
                table: "Obstacles",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ObstacleID",
                table: "Obstacles",
                newName: "Id");

            migrationBuilder.AddColumn<double>(
                name: "Height",
                table: "Obstacles",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "Obstacles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Obstacles",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Obstacles",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "Obstacles");

            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "Obstacles");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Obstacles");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Obstacles");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Obstacles",
                newName: "ObstacleName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Obstacles",
                newName: "ObstacleDescription");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Obstacles",
                newName: "ObstacleCoords");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Obstacles",
                newName: "ObstacleID");

            migrationBuilder.AddColumn<int>(
                name: "ObstacleHeight",
                table: "Obstacles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
