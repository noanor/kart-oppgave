using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Luftfartshinder.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationContext context;

        public ReportRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<Report> AddAsync(Report report)
        {
            await context.Reports.AddAsync(report);
            await context.SaveChangesAsync();
            return report;
        }

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await context.Reports.Include(r => r.Obstacles).ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await context.Reports.Include(r => r.Obstacles).FirstAsync(r => r.Id == id);
        }

        public async Task<Report?> UpdateAsync(Report report)
        {
            var existingReport = await context.Reports.FindAsync(report.Id);
            if (existingReport != null)
            {
                existingReport.Title = report.Title;

                await context.SaveChangesAsync();

                return existingReport;
            }
            return null;
        }
    }
}
