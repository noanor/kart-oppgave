using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Luftfartshinder.Repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly AuthDbContext _authDbContext;

        public OrganizationRepository(AuthDbContext context)
        {
            _authDbContext = context;
        }

        public async Task<Organization> Add(Organization organization)
        {
            await _authDbContext.AddAsync(organization);
            await _authDbContext.SaveChangesAsync();
            return organization;
        }

        public async Task<List<Organization>> GetAll()
        {
            return await _authDbContext.Organizations.ToListAsync();
        }
        public async Task<Organization?> GetById(int id)
        {
            return await _authDbContext.Organizations.FindAsync(id);
        }

        public async Task<Organization?> GetByName(string orgName)
        {
            return await _authDbContext.Organizations
                        .FirstOrDefaultAsync(o => o.Name == orgName);
        }
    }
}
