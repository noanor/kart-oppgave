using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luftfartshinder.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class InitialAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1d3b44cf-5507-444f-b84c-842539f13e02",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "c1731d70-d186-4d72-a4fe-0d5250717f2e", "414f24f4-bc20-4099-9f49-99595182027c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "322acd53-a201-47c6-a7e0-6695690ce677",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "aadc510e-8578-48b5-961c-af902eb37548", "caf52324-250b-432b-ad53-1a119a844b05" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3c1b1dcf-6345-42b9-90fe-45227eb5be5b",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "d1c69796-cb8d-4ec4-82b2-edc61e46f501", "d6178404-3ba1-4d80-8bee-00b5a34090ce" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1d3b44cf-5507-444f-b84c-842539f13e02",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "daf120b9-dd54-4821-99e4-96589633b849", "03218dda-e20a-49dc-8686-f0d9c9da94a0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "322acd53-a201-47c6-a7e0-6695690ce677",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "9200be15-b2cc-49c2-8bd9-1000c8c5c471", "87eca17a-7b07-451d-96f9-49d88e3312a0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3c1b1dcf-6345-42b9-90fe-45227eb5be5b",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "1ec9e4cd-32f2-4445-acfc-b7991c322410", "673af29c-78a0-401b-a649-428981c2b51f" });
        }
    }
}
