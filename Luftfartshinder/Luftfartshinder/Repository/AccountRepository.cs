using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Luftfartshinder.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationContext context;


        public AccountRepository(ApplicationContext context)
        {
            this.context = context;

        }

        public List<Report> GetUserReports(string userId)
        {
            var reports = context.Reports.Include(r => r.Obstacles).Where(r => r.AuthorId == userId).ToList();
            return reports;
        }
    }
}
