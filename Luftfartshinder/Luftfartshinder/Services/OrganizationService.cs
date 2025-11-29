using Luftfartshinder.Models.Domain;
using Luftfartshinder.Repository;

namespace Luftfartshinder.Services
{
    /// <summary>
    /// Service implementation for organization-related business logic.
    /// </summary>
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository organizationRepository;
        private const string KartverketName = "Kartverket";

        public OrganizationService(IOrganizationRepository organizationRepository)
        {
            this.organizationRepository = organizationRepository;
        }

        /// <summary>
        /// Gets or creates an organization based on role and provided organization name.
        /// </summary>
        public async Task<Organization?> GetOrCreateOrganizationForRoleAsync(string role, string? organizationName, string? otherOrganizationName)
        {
            if (role == "FlightCrew")
            {
                return await GetOrCreateFlightCrewOrganizationAsync(organizationName, otherOrganizationName);
            }
            else if (role == "Registrar")
            {
                return await GetOrCreateKartverketAsync();
            }

            return null;
        }

        private async Task<Organization?> GetOrCreateFlightCrewOrganizationAsync(string? organizationName, string? otherOrganizationName)
        {
            string? orgName = null;

            if (organizationName == "Other" && !string.IsNullOrWhiteSpace(otherOrganizationName))
            {
                orgName = otherOrganizationName;
            }
            else
            {
                orgName = organizationName;
            }

            if (string.IsNullOrWhiteSpace(orgName))
            {
                return null;
            }

            var organization = await organizationRepository.GetByName(orgName);
            if (organization == null)
            {
                organization = new Organization { Name = orgName };
                await organizationRepository.Add(organization);
            }

            return organization;
        }

        private async Task<Organization> GetOrCreateKartverketAsync()
        {
            var organization = await organizationRepository.GetByName(KartverketName);
            if (organization == null)
            {
                organization = new Organization { Name = KartverketName };
                await organizationRepository.Add(organization);
            }
            return organization;
        }
    }
}

