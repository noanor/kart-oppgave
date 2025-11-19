using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Repository
{
    public interface IAccountRepository
    {
        public List<Report> GetUserReports(string userId);
    }
}
