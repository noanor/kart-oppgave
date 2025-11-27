using Luftfartshinder.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace Luftfartshinder.Services
{
    /// <summary>
    /// Service for handling user-related business logic.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Validates if email and username are available.
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateUserAsync(string email, string username);

        /// <summary>
        /// Creates a user with password and assigns a role.
        /// </summary>
        Task<(bool Success, IdentityResult? Result)> CreateUserAsync(ApplicationUser user, string password, string role);
    }
}

