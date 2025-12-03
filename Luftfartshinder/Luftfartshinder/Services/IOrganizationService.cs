using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Services
{
    /// <summary>
    /// Service for handling organization-related business logic.
    /// </summary>
    public interface IOrganizationService
    {
        /// <summary>
        /// Gets or creates an organization based on role and provided organization name.
        /// </summary>
        Task<Organization?> GetOrCreateOrganizationForRoleAsync(string role, string? organizationName, string? otherOrganizationName);
    }
}

