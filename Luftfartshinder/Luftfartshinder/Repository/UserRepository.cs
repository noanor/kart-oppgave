using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Luftfartshinder.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext authDbContext;

        public UserRepository(AuthDbContext authDbContext)
        {
            this.authDbContext = authDbContext;
        }
        public async Task<IEnumerable<ApplicationUser>> GetAll()
        {
            var users = await authDbContext.Users
                .Include(u => u.Organization)
                .ToListAsync();
            
            var superAdminUser = users.FirstOrDefault(x => x.Email == "superadmin@kartverket.no");
            if (superAdminUser != null)
            {
                users.Remove(superAdminUser);
            }
            return users;
        }
    }
}

