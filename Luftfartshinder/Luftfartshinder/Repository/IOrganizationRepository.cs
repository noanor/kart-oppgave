using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Repository
{
    public interface IOrganizationRepository
    {
        List<Organization> GetAll();
        Task<Organization?> GetById(int id);
        Task<Organization?> GetByName(string orgName);
        Task<Organization> Add(Organization organization);
        
    }
}
