using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Luftfartshinder.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _authDbContext;

        public UserRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }
        public async Task<IEnumerable<ApplicationUser>> GetAll()
        {
            var users = await _authDbContext.Users
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

