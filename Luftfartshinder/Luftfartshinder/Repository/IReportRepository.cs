using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Repository
{
    public interface IReportRepository
    {
        Task<List<Report>> GetAllAsync();
        Task<Report?> GetByIdAsync(int id);
        Task<Report?> UpdateAsync(Report report);
        Task<Report> AddAsync(Report report);
        Task<Report?> DeleteAsync(int id);
        Task<List<Report>> GetByOrgId(int organizationId);
    }
}

