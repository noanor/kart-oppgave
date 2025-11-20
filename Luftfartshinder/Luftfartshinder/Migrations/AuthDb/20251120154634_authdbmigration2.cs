using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luftfartshinder.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class authdbmigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3c1b1dcf-6345-42b9-90fe-45227eb5be5b",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "bf060dbb-f309-4295-afea-ad694a945fa0", "d35b80e2-0eab-44f7-b5c7-3e1b6d25688f" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsApproved", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "Organization", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1d3b44cf-5507-444f-b84c-842539f13e02", 0, "0cb27bac-8f93-40b8-8835-a1e4d95a1a13", "pilot@kartverket.no", false, "Kaptein", true, "Pilot", false, null, "PILOT@KARTVERKET.NO", "PILOT", null, "AQAAAAIAAYagAAAAEKK/tjn9DmfSvd9EhZ1uGpB4grNXZ3L4D07PdU+vRm2QBPdbMk5G1OiekqX1C4B2PA==", null, false, "59a10fb2-ae05-434f-89de-3d9767f37fb2", false, "pilot" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "d0fe1bc1-1838-48db-b483-a31510e5a2f6", "1d3b44cf-5507-444f-b84c-842539f13e02" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "d0fe1bc1-1838-48db-b483-a31510e5a2f6", "1d3b44cf-5507-444f-b84c-842539f13e02" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1d3b44cf-5507-444f-b84c-842539f13e02");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3c1b1dcf-6345-42b9-90fe-45227eb5be5b",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "ade99e60-ec20-4977-b153-f87f84791676", "63f4c9f8-aa59-4467-8da6-c2bb9b57f6c3" });
        }
    }
}
