<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251120091742_authmigration.cs
<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251119101206_authmig.cs
<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251118164323_authdbmigration.cs
<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251117183933_AuthDbMig.cs
<<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251119092435_authdbmigration.cs
=======
>>>>>>> 8bf41d3 (Fungerende approve og reject knapper registrar details page):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251118164323_authdbmigration.cs
﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
=======
﻿using Microsoft.EntityFrameworkCore.Metadata;
>>>>>>> 985ea00 (Delete, Approve, Reject, Save Note):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251119101206_authmig.cs
=======
﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
>>>>>>> 9be7fd2 (La til flere toasts og modals):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251120152955_authdbmigration.cs
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Luftfartshinder.Migrations.AuthDb
{
    /// <inheritdoc />
<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251120091742_authmigration.cs
    public partial class authmig : Migration
=======
    public partial class authdbmigration : Migration
>>>>>>> 9be7fd2 (La til flere toasts og modals):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251120152955_authdbmigration.cs
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsApproved = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Organization = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "66eeb3d3-c3a2-4c2a-8e47-d6513739f417", "66eeb3d3-c3a2-4c2a-8e47-d6513739f417", "SuperAdmin", "SUPERADMIN" },
                    { "89b2d41d-faa8-45fe-8601-1925778c4c30", "89b2d41d-faa8-45fe-8601-1925778c4c30", "Registrar", "REGISTRAR" },
                    { "d0fe1bc1-1838-48db-b483-a31510e5a2f6", "d0fe1bc1-1838-48db-b483-a31510e5a2f6", "FlightCrew", "FLIGHTCREW" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsApproved", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "Organization", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251120091742_authmigration.cs
<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251118164323_authdbmigration.cs
<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251117183933_AuthDbMig.cs
                values: new object[] { "3c1b1dcf-6345-42b9-90fe-45227eb5be5b", 0, "a95ce37b-4ab4-4b0c-8f09-dcc6d21dea1f", "superadmin@kartverket.no", false, "Super", true, "Admin", false, null, "SUPERADMIN@KARTVERKET.NO", "SUPERADMIN@KARTVERKET.NO", null, "AQAAAAIAAYagAAAAEH47+CKFibjiheWX+ESu0lWsKk2kMdbDeq0/1uuZRKqLw+a8CzqP/mDnVKJl7/Kq8A==", null, false, "d007283e-d252-4447-8610-43c161dce390", false, "superadmin@kartverket.no" });
=======
                values: new object[] { "3c1b1dcf-6345-42b9-90fe-45227eb5be5b", 0, "ee154f8b-bf64-4af0-87fb-5ec9e8538dba", "superadmin@kartverket.no", false, "Super", true, "Admin", false, null, "SUPERADMIN@KARTVERKET.NO", "SUPERADMIN@KARTVERKET.NO", null, "AQAAAAIAAYagAAAAEH47+CKFibjiheWX+ESu0lWsKk2kMdbDeq0/1uuZRKqLw+a8CzqP/mDnVKJl7/Kq8A==", null, false, "15e58774-a927-44b5-8747-a23f7fac098d", false, "superadmin@kartverket.no" });
>>>>>>> 8bf41d3 (Fungerende approve og reject knapper registrar details page):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251118164323_authdbmigration.cs
=======
                values: new object[] { "3c1b1dcf-6345-42b9-90fe-45227eb5be5b", 0, "dc6a9db0-e9e4-45ce-b55a-dec9fa9f5073", "superadmin@kartverket.no", false, "Super", true, "Admin", false, null, "SUPERADMIN@KARTVERKET.NO", "SUPERADMIN@KARTVERKET.NO", null, "AQAAAAIAAYagAAAAEH47+CKFibjiheWX+ESu0lWsKk2kMdbDeq0/1uuZRKqLw+a8CzqP/mDnVKJl7/Kq8A==", null, false, "04831698-e60f-43af-a6e2-d84b20d53fca", false, "superadmin@kartverket.no" });
>>>>>>> 985ea00 (Delete, Approve, Reject, Save Note):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251119101206_authmig.cs
=======
                values: new object[] { "3c1b1dcf-6345-42b9-90fe-45227eb5be5b", 0, "ade99e60-ec20-4977-b153-f87f84791676", "superadmin@kartverket.no", false, "Super", true, "Admin", false, null, "SUPERADMIN@KARTVERKET.NO", "SUPERADMIN@KARTVERKET.NO", null, "AQAAAAIAAYagAAAAEH47+CKFibjiheWX+ESu0lWsKk2kMdbDeq0/1uuZRKqLw+a8CzqP/mDnVKJl7/Kq8A==", null, false, "63f4c9f8-aa59-4467-8da6-c2bb9b57f6c3", false, "superadmin@kartverket.no" });
>>>>>>> 9be7fd2 (La til flere toasts og modals):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251120152955_authdbmigration.cs

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "66eeb3d3-c3a2-4c2a-8e47-d6513739f417", "3c1b1dcf-6345-42b9-90fe-45227eb5be5b" },
                    { "89b2d41d-faa8-45fe-8601-1925778c4c30", "3c1b1dcf-6345-42b9-90fe-45227eb5be5b" },
                    { "d0fe1bc1-1838-48db-b483-a31510e5a2f6", "3c1b1dcf-6345-42b9-90fe-45227eb5be5b" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251120091742_authmigration.cs
<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251117183933_AuthDbMig.cs
========
=======
>>>>>>> 1c7ee01 (La til success message (toast) på index, markers blir på kartet til draft er submitted. Litt registrar refaktorisering):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251120091742_authmigration.cs
﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Luftfartshinder.Migrations.AuthDb
{
    /// <inheritdoc />
<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251119101206_authmig.cs
    public partial class AuthDbMig : Migration
=======
    public partial class authmigration : Migration
>>>>>>> 1c7ee01 (La til success message (toast) på index, markers blir på kartet til draft er submitted. Litt registrar refaktorisering):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251120091742_authmigration.cs
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsApproved = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Organization = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "66eeb3d3-c3a2-4c2a-8e47-d6513739f417", "66eeb3d3-c3a2-4c2a-8e47-d6513739f417", "SuperAdmin", "SUPERADMIN" },
                    { "89b2d41d-faa8-45fe-8601-1925778c4c30", "89b2d41d-faa8-45fe-8601-1925778c4c30", "Registrar", "REGISTRAR" },
                    { "d0fe1bc1-1838-48db-b483-a31510e5a2f6", "d0fe1bc1-1838-48db-b483-a31510e5a2f6", "FlightCrew", "FLIGHTCREW" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsApproved", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "Organization", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251119101206_authmig.cs
                values: new object[] { "3c1b1dcf-6345-42b9-90fe-45227eb5be5b", 0, "401bfa3c-e784-48d5-abf2-44184f4155f4", "superadmin@kartverket.no", false, "Super", true, "Admin", false, null, "SUPERADMIN@KARTVERKET.NO", "SUPERADMIN@KARTVERKET.NO", null, "AQAAAAIAAYagAAAAEH47+CKFibjiheWX+ESu0lWsKk2kMdbDeq0/1uuZRKqLw+a8CzqP/mDnVKJl7/Kq8A==", null, false, "49b3d5ca-5838-4870-a6cb-eee0a4d98f2c", false, "superadmin@kartverket.no" });
=======
                values: new object[] { "3c1b1dcf-6345-42b9-90fe-45227eb5be5b", 0, "009f29c6-becd-435a-bea1-737aba28dad5", "superadmin@kartverket.no", false, "Super", true, "Admin", false, null, "SUPERADMIN@KARTVERKET.NO", "SUPERADMIN@KARTVERKET.NO", null, "AQAAAAIAAYagAAAAEH47+CKFibjiheWX+ESu0lWsKk2kMdbDeq0/1uuZRKqLw+a8CzqP/mDnVKJl7/Kq8A==", null, false, "5d87a9be-dd39-4f67-853f-dcc0fdf41f57", false, "superadmin@kartverket.no" });
>>>>>>> 1c7ee01 (La til success message (toast) på index, markers blir på kartet til draft er submitted. Litt registrar refaktorisering):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251120091742_authmigration.cs

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "66eeb3d3-c3a2-4c2a-8e47-d6513739f417", "3c1b1dcf-6345-42b9-90fe-45227eb5be5b" },
                    { "89b2d41d-faa8-45fe-8601-1925778c4c30", "3c1b1dcf-6345-42b9-90fe-45227eb5be5b" },
                    { "d0fe1bc1-1838-48db-b483-a31510e5a2f6", "3c1b1dcf-6345-42b9-90fe-45227eb5be5b" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
<<<<<<< HEAD:Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251119101206_authmig.cs
>>>>>>>> 1f64686 (La til kart preview på registrar details side):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251117183933_AuthDbMig.cs
=======
>>>>>>> 8bf41d3 (Fungerende approve og reject knapper registrar details page):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251118164323_authdbmigration.cs
=======
>>>>>>> 1c7ee01 (La til success message (toast) på index, markers blir på kartet til draft er submitted. Litt registrar refaktorisering):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251120091742_authmigration.cs
=======
>>>>>>> 9be7fd2 (La til flere toasts og modals):Luftfartshinder/Luftfartshinder/Migrations/AuthDb/20251120152955_authdbmigration.cs
