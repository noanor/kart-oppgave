using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luftfartshinder.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class AddUserApprovalColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE AspNetUsers 
                ADD COLUMN IF NOT EXISTS IsApproved TINYINT(1) NOT NULL DEFAULT 0;
            ");

            migrationBuilder.Sql(@"
                UPDATE AspNetUsers 
                SET IsApproved = 1 
                WHERE Email = 'superadmin@kartverket.no';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3c1b1dcf-6345-42b9-90fe-45227eb5be5b",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "de019b91-47cf-4ec2-8f1e-bae9327777f7", "AQAAAAIAAYagAAAAEBC4kTs+Gl9ZwnxYRase9y8vcKN7eufS9m0xYYmSNGaGpZsDUEtX6dMLg1HG3oZ2Xw==", "122b8647-de8f-414f-a07c-d629f6f5d389" });
        }
    }
}
