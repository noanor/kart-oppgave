using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Luftfartshinder.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationContext _context;

        public AccountRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<Report>> GetUserReports(string userId)
        {
            return await _context.Reports
                .Include(r => r.Obstacles)
                .Where(r => r.AuthorId == userId)
                .ToListAsync();
        }
    }
}
