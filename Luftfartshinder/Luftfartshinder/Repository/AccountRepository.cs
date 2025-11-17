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

        public async Task<IEnumerable<Report>> GetReports()
        {
            var reports = await reportRepository.GetAllAsync();
            return reports;
        }
    }
}
