using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luftfartshinder.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class authmigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1d3b44cf-5507-444f-b84c-842539f13e02",
                columns: new[] { "ConcurrencyStamp", "Organization", "SecurityStamp" },
                values: new object[] { "62bc6710-45b9-4540-8aa2-1e5cb29c0220", "Norwegian Armed Forces", "d6aeeeaa-aefc-4da7-94a3-d40a6720c43f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3c1b1dcf-6345-42b9-90fe-45227eb5be5b",
                columns: new[] { "ConcurrencyStamp", "Organization", "SecurityStamp" },
                values: new object[] { "f784d96b-8d93-4d2b-9531-b0eca352bc4e", "Kartverket", "52da109a-7fec-49f0-bf04-60db305a36b5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1d3b44cf-5507-444f-b84c-842539f13e02",
                columns: new[] { "ConcurrencyStamp", "Organization", "SecurityStamp" },
                values: new object[] { "3640ea2f-ddda-4112-bbcd-e8b171cf2ae0", null, "1679493f-7a90-409c-8d8e-ea4b4758e2bb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3c1b1dcf-6345-42b9-90fe-45227eb5be5b",
                columns: new[] { "ConcurrencyStamp", "Organization", "SecurityStamp" },
                values: new object[] { "3cbb98b4-3f5a-4b55-9c6b-34e651834e99", null, "bd4a01f5-87b6-4a4b-aa7f-cea1d375959c" });
        }
    }
}
