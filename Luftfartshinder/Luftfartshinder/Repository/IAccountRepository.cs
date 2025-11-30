using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Repository
{
    public interface IAccountRepository
    {
        Task<List<Report>> GetUserReports(string userId);
    }
}
