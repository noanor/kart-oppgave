using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Luftfartshinder.Migrations.Application
{
    /// <inheritdoc />
    public partial class SeedSecondPilotObstacles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReportDate",
                value: new DateTime(2025, 11, 30, 9, 40, 1, 243, DateTimeKind.Utc).AddTicks(4435));

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReportDate",
                value: new DateTime(2025, 12, 2, 9, 40, 1, 243, DateTimeKind.Utc).AddTicks(4659));

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReportDate",
                value: new DateTime(2025, 12, 4, 9, 40, 1, 243, DateTimeKind.Utc).AddTicks(4664));

            migrationBuilder.InsertData(
                table: "Reports",
                columns: new[] { "Id", "Author", "AuthorId", "OrganizationId", "ReportDate", "Summary", "Title" },
                values: new object[,]
                {
                    { 4, "pilot2", "2e4c55df-6618-5550-c95d-953640f24e13", 4, new DateTime(2025, 12, 3, 9, 40, 1, 243, DateTimeKind.Utc).AddTicks(4666), "Obstacles identified during emergency flight route to Tromsø", "Northern Route Obstacle Survey - Tromsø" },
                    { 5, "pilot2", "2e4c55df-6618-5550-c95d-953640f24e13", 4, new DateTime(2025, 12, 1, 9, 40, 1, 243, DateTimeKind.Utc).AddTicks(4668), "Powerline obstacles in Stavanger region affecting flight paths", "Stavanger Area Powerline Report" }
                });

            migrationBuilder.InsertData(
                table: "Obstacles",
                columns: new[] { "Id", "Description", "Height", "Latitude", "Longitude", "Name", "OrganizationId", "RegistrarNote", "ReportId", "Status", "Type" },
                values: new object[,]
                {
                    { 9, "Communication tower near Tromsø airport", 85.0, 69.649199999999993, 18.955300000000001, "Communication Tower", 4, null, 4, 0, "mast" },
                    { 10, "Weather monitoring mast in Tromsø region", 20.0, 69.650000000000006, 18.960000000000001, "Weather Mast", 4, null, 4, 0, "point" },
                    { 11, "High voltage powerline crossing flight path", 50.0, 69.644999999999996, 18.949999999999999, "High Voltage Line", 4, null, 4, 1, "powerline" },
                    { 12, "Main powerline in Stavanger area", 40.0, 58.969999999999999, 5.7331000000000003, "Main Powerline", 4, null, 5, 0, "powerline" },
                    { 13, "Overhead cable line near Stavanger", 28.5, 58.975000000000001, 5.7400000000000002, "Cable Line", 4, null, 5, 0, "line" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReportDate",
                value: new DateTime(2025, 11, 30, 9, 27, 14, 420, DateTimeKind.Utc).AddTicks(9261));

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReportDate",
                value: new DateTime(2025, 12, 2, 9, 27, 14, 420, DateTimeKind.Utc).AddTicks(9538));

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReportDate",
                value: new DateTime(2025, 12, 4, 9, 27, 14, 420, DateTimeKind.Utc).AddTicks(9543));
        }
    }
}
