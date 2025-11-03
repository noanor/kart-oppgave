using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luftfartshinder.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class OppdaterteSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "A5ABD9EB-0D52-42BF-9655-2CD12BAB8331",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "afe07566-d4b9-44a2-8acb-3339ac02b8bd", "superAdmin1@kartverket.no", "SUPERADMIN1@KARTVERKET.NO", "SUPERADMIN1@KARTVERKET.NO", "AQAAAAIAAYagAAAAEPxsi3gHbHL84JZ3rrjrqzdnyZ0HM/uAYYzNCyjkGluo7t4Ixnc+47KUNcdV/uAS0A==", "cdf9a32e-b8e1-44c9-a25a-598bdd37c5a2", "superAdmin1@kartverket.no" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "A5ABD9EB-0D52-42BF-9655-2CD12BAB8331",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "3f816ae6-74f1-4836-bb81-c3106e9d558d", "superAdmin1@nla.no", "SUPERADMIN1@NLA.NO", "SUPERADMIN1@NLA.NO", "AQAAAAIAAYagAAAAEPx2ezyYYjfKg9AgVWFG1v7ppwPoWPcSzIPayTCClvKP38P5ahFIhT8VB1164cmMDA==", "a9a76dda-666a-47a3-9ec6-505832415198", "superAdmin1@nla.no" });
        }
    }
}
