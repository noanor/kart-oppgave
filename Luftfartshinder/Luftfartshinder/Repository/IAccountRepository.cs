using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Repository
{
    public interface IAccountRepository
    {
        public Task<Report> GetReports();
    }
}
