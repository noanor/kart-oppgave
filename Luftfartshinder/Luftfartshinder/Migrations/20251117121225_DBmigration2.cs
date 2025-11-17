using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luftfartshinder.Migrations
{
    /// <inheritdoc />
    public partial class DBmigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegistrarNote",
                table: "Reports",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrarNote",
                table: "Reports");
        }
    }
}
