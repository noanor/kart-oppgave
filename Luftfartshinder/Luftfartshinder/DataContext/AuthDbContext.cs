using Luftfartshinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Luftfartshinder.DataContext
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //seed roller

            var flightCrewRoleId = "d0fe1bc1-1838-48db-b483-a31510e5a2f6";
            var registrarRoleId = "89b2d41d-faa8-45fe-8601-1925778c4c30";
            var superAdminRoleId = "66eeb3d3-c3a2-4c2a-8e47-d6513739f417";

            var roller = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "FlightCrew",
                    NormalizedName = "FLIGHTCREW",
                    Id = flightCrewRoleId,
                    ConcurrencyStamp = flightCrewRoleId
                },
                new IdentityRole
                {
                    Name = "Registrar",
                    NormalizedName = "REGISTRAR",
                    Id = registrarRoleId,
                    ConcurrencyStamp = registrarRoleId
                },
                new IdentityRole
                {
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    Id = superAdminRoleId,
                    ConcurrencyStamp = superAdminRoleId
                }
            };

            //Gjør klar for migrasjon
            builder.Entity<IdentityRole>().HasData(roller);

            //SuperAdmin Bruker, Identity Bruker + Pilot bruker
            var superAdminId = "3c1b1dcf-6345-42b9-90fe-45227eb5be5b";
            var pilotId = "1d3b44cf-5507-444f-b84c-842539f13e02";

            var superAdminUser = new ApplicationUser
            {
                Id = superAdminId,
                UserName = "superadmin@kartverket.no",
                NormalizedUserName = "SUPERADMIN@KARTVERKET.NO",
                Email = "superadmin@kartverket.no",
                NormalizedEmail = "SUPERADMIN@KARTVERKET.NO",
                FirstName = "Super",
                LastName = "Admin",
                IsApproved = true
            };

            var pilotUser = new ApplicationUser
            {
                Id = pilotId,
                UserName = "pilot",
                NormalizedUserName = "PILOT",
                Email = "pilot@kartverket.no",
                NormalizedEmail = "PILOT@KARTVERKET.NO",
                FirstName = "Kaptein",
                LastName = "Pilot",
                IsApproved = true
            };

            superAdminUser.PasswordHash = "AQAAAAIAAYagAAAAEH47+CKFibjiheWX+ESu0lWsKk2kMdbDeq0/1uuZRKqLw+a8CzqP/mDnVKJl7/Kq8A==";
            pilotUser.PasswordHash = "AQAAAAIAAYagAAAAEKK/tjn9DmfSvd9EhZ1uGpB4grNXZ3L4D07PdU+vRm2QBPdbMk5G1OiekqX1C4B2PA==";

            builder.Entity<ApplicationUser>().HasData(superAdminUser);
            builder.Entity<ApplicationUser>().HasData(pilotUser);




            // Gi alle rollene til SuperAdmin
            var superAdminRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    RoleId = flightCrewRoleId,
                    UserId = superAdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = registrarRoleId,
                    UserId = superAdminId
                },
                 new IdentityUserRole<string>
                {
                    RoleId = superAdminRoleId,
                    UserId = superAdminId
                }
            };

            // Gi pilot rolle til pilotbruker
            var pilotRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    RoleId = flightCrewRoleId,
                    UserId = pilotId
                }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(superAdminRoles);
            builder.Entity<IdentityUserRole<string>>().HasData(pilotRoles);
        }
    }
}

