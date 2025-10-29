using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Luftfartshinder.DataContext
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //seed roles(FlyBesetning, RegisterFører, SuperAdmin)

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
                    NormalizedName = "Registerfører"
                    Id = registerFørerRolleId,
                    ConcurrencyStamp = registerFørerRolleId
                },
                new IdentityRole
                {
                    Name = "Superadmin"
                    NormalizedName = "Superadmin"
                    Id = superAdminRolleId,
                    ConcurrencyStamp = superAdminRolleId
                }
            };
        }
    }
}

