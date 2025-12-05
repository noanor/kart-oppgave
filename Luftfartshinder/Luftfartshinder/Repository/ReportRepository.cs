using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Luftfartshinder.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationContext _context;

        public ReportRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Report> AddAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<List<Report>> GetAllAsync()
        {
            return await _context.Reports.Include(r => r.Obstacles).ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports.Include(r => r.Obstacles).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Report>> GetByOrgId(int organizationId)
        {
            return await _context.Reports
                .Where(r => r.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<Report?> UpdateAsync(Report report)
        {
            var existingReport = await _context.Reports.FindAsync(report.Id);
            if (existingReport != null)
            {
                existingReport.Title = report.Title;
                existingReport.Summary = report.Summary;

                await _context.SaveChangesAsync();

                return existingReport;
            }
            return null;
        }

        public async Task<Report?> DeleteAsync(int id)
        {
            var existingReport = await _context.Reports
                .Include(r => r.Obstacles)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingReport != null)
            {
                _context.Obstacles.RemoveRange(existingReport.Obstacles);
                _context.Reports.Remove(existingReport);

                await _context.SaveChangesAsync();
                return existingReport;
            }

            return null;
        }
    }
}
