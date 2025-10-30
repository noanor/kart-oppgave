using Luftfartshinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.IO.Pipelines;

namespace Luftfartshinder.DataContext
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //seed roller

            var flyBesetningRolleId = "d0fe1bc1-1838-48db-b483-a31510e5a2f6";
            var registerFørerRolleId = "89b2d41d-faa8-45fe-8601-1925778c4c30";
            var superAdminRolleId = "66eeb3d3-c3a2-4c2a-8e47-d6513739f417";

            var roller = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Flybesetning",
                    NormalizedName = "Flybesetning",
                    Id = flyBesetningRolleId,
                    ConcurrencyStamp = flyBesetningRolleId
                },
                new IdentityRole
                {
                    Name = "Registerfører",
                    NormalizedName = "Registerfører",
                    Id = registerFørerRolleId,
                    ConcurrencyStamp = registerFørerRolleId
                },
                new IdentityRole
                {
                    Name = "Superadmin",
                    NormalizedName = "Superadmin",
                    Id = superAdminRolleId,
                    ConcurrencyStamp = superAdminRolleId
                }
            };
            
            //Gjør klar for migrasjon
            builder.Entity<IdentityRole>().HasData(roller);

            //SuperAdmin Bruker, Identity Bruker

            var superAdminId = "3c1b1dcf-6345-42b9-90fe-45227eb5be5b";

            var superAdminBruker = new IdentityUser
            {
                Id = superAdminId,
                UserName = "superadmin@kartverket.no",
                NormalizedUserName = "Superadmin@kartverket.no".ToUpper(),
                Email = "superadmin@kartverket.no",
                NormalizedEmail = "Superadmin@kartverket.no".ToUpper(),
               
               
            };

            superAdminBruker.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(superAdminBruker, "Superadmin123");

            builder.Entity<IdentityUser>().HasData(superAdminBruker);

       

            // Gi alle brukerne til SuperAdmin,

            var superAdminRoller = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    RoleId = flyBesetningRolleId,
                    UserId = superAdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = registerFørerRolleId,
                    UserId = superAdminId
                },
                 new IdentityUserRole<string>
                {
                    RoleId = superAdminRolleId,
                    UserId = superAdminId
                }
            };
            builder.Entity<IdentityUserRole<string>>().HasData(superAdminRoller);
        }
    }
}

