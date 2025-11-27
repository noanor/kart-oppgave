using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Repository
{
    public interface IOrganizationRepository
    {
        Task<List<Organization>> GetAll();
        Task<Organization?> GetById(int id);
        Task<Organization?> GetByName(string orgName);
        Task<Organization> Add(Organization organization);
        
    }
}
