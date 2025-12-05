using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Luftfartshinder.Migrations.Application
{
    /// <inheritdoc />
    public partial class SeedObstaclesAndReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Obstacles",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Obstacles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Reports",
                columns: new[] { "Id", "Author", "AuthorId", "OrganizationId", "ReportDate", "Summary", "Title" },
                values: new object[,]
                {
                    { 1, "pilot", "1d3b44cf-5507-444f-b84c-842539f13e02", 4, new DateTime(2025, 11, 30, 9, 27, 14, 420, DateTimeKind.Utc).AddTicks(9261), "Multiple powerline obstacles detected during flight survey", "Powerline Obstacle Report - Oslo Area" },
                    { 2, "pilot", "1d3b44cf-5507-444f-b84c-842539f13e02", 4, new DateTime(2025, 12, 2, 9, 27, 14, 420, DateTimeKind.Utc).AddTicks(9538), "Communication masts and towers identified in flight path", "Mast and Tower Report - Bergen Region" },
                    { 3, "registrar", "322acd53-a201-47c6-a7e0-6695690ce677", 1, new DateTime(2025, 12, 4, 9, 27, 14, 420, DateTimeKind.Utc).AddTicks(9543), "Various urban obstacles including buildings and structures", "Urban Obstacle Survey - Trondheim" }
                });

            migrationBuilder.InsertData(
                table: "Obstacles",
                columns: new[] { "Id", "Description", "Height", "Latitude", "Longitude", "Name", "OrganizationId", "RegistrarNote", "ReportId", "Status", "Type" },
                values: new object[,]
                {
                    { 1, "High voltage powerline crossing flight path near Oslo", 45.5, 59.913899999999998, 10.7522, "High Voltage Powerline", 4, null, 1, 0, "powerline" },
                    { 2, "Distribution powerline near residential area in Oslo", 25.0, 59.920000000000002, 10.76, "Distribution Line", 4, null, 1, 0, "powerline" },
                    { 3, "Overhead cable line in Oslo region", 30.0, 59.914999999999999, 10.755000000000001, "Cable Line", 4, null, 1, 1, "line" },
                    { 4, "Tall communication mast in Bergen area", 120.0, 60.391300000000001, 5.3220999999999998, "Communication Mast", 4, null, 2, 0, "mast" },
                    { 5, "Radio transmission tower near Bergen", 95.5, 60.399999999999999, 5.3300000000000001, "Radio Tower", 4, null, 2, 0, "mast" },
                    { 6, "Weather monitoring station in Bergen region", 15.0, 60.390000000000001, 5.3200000000000003, "Weather Station Mast", 4, null, 2, 2, "point" },
                    { 7, "Antenna on top of building in Trondheim", 12.5, 63.430500000000002, 10.395099999999999, "Building Antenna", 1, null, 3, 0, "point" },
                    { 8, "Large industrial building complex in Trondheim", 35.0, 63.435000000000002, 10.4, "Industrial Complex", 1, null, 3, 0, "area" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Obstacles",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Obstacles",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Obstacles",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
