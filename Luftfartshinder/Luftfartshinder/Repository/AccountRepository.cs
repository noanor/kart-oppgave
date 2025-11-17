using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationContext context;
        private readonly IReportRepository reportRepository;

        public AccountRepository(ApplicationContext context, IReportRepository reportRepository)
        {
            this.context = context;
            this.reportRepository = reportRepository;
        }

        public Task<Report> GetReports()
        {
            throw new NotImplementedException();
        }
    }
}
