using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Luftfartshinder.Repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly AuthDbContext context;

        public OrganizationRepository(AuthDbContext context)
        {
            this.context = context;
        }

        public async Task<Organization> Add(Organization organization)
        {
            await context.AddAsync(organization);
            await context.SaveChangesAsync();
            return organization;
        }

        public List<Organization> GetAll()
        {
            return context.Organizations.ToList();
        }
        public async Task<Organization?> GetById(int id)
        {
            var existingOrg = await context.Organizations.FindAsync(id);
            if (existingOrg == null)
            {
                return null;
            }
            return existingOrg;
        }

        public async Task<Organization?> GetByName(string orgName)
        {
            var existingOrg = await context.Organizations
                        .FirstOrDefaultAsync(o => o.Name == orgName);

            if(existingOrg == null)
            {
                return null;
            }
            return existingOrg;
        }
    }
}
