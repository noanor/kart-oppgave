using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luftfartshinder.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class SeedSecondPilot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1d3b44cf-5507-444f-b84c-842539f13e02",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "37072807-23f8-4d68-be3e-d8286673dee0", "15a96e31-e9ca-4f24-b6df-eef103708890" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "322acd53-a201-47c6-a7e0-6695690ce677",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "b1f02907-5bc1-4ded-8baa-8fa8dc5e18c4", "21ab13f7-38c5-4a5b-9638-9edff2bf2131" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3c1b1dcf-6345-42b9-90fe-45227eb5be5b",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "c68c0d2b-a9ed-445b-97c9-d0e9117f7769", "11756974-ef37-4616-8d32-aa8876ba3607" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsApproved", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OrganizationId", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "2e4c55df-6618-5550-c95d-953640f24e13", 0, "51b83108-10ea-4ff1-be6b-efb4f27828a4", "pilot2@airambulance.no", false, "Anna", true, "Hansen", false, null, "PILOT2@AIRAMBULANCE.NO", "PILOT2", 4, "AQAAAAIAAYagAAAAEKK/tjn9DmfSvd9EhZ1uGpB4grNXZ3L4D07PdU+vRm2QBPdbMk5G1OiekqX1C4B2PA==", null, false, "9e000dfd-3608-4daf-893e-039b9865bd78", false, "pilot2" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "d0fe1bc1-1838-48db-b483-a31510e5a2f6", "2e4c55df-6618-5550-c95d-953640f24e13" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "d0fe1bc1-1838-48db-b483-a31510e5a2f6", "2e4c55df-6618-5550-c95d-953640f24e13" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2e4c55df-6618-5550-c95d-953640f24e13");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1d3b44cf-5507-444f-b84c-842539f13e02",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "0bfed2b7-28f9-4d6f-8091-00a175815758", "bbc13c99-5ddf-4806-bf5c-0249fbe67c46" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "322acd53-a201-47c6-a7e0-6695690ce677",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "4441d56a-db05-490d-869a-e6947d3917a0", "3a3c9c08-9a40-4c39-8f2f-754147377188" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3c1b1dcf-6345-42b9-90fe-45227eb5be5b",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "7845d8e2-f712-4112-8bdb-899e27d22c74", "caf82c13-ecd0-409c-98b2-0084bac49974" });
        }
    }
}
