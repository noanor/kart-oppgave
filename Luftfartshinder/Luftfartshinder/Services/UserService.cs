using Luftfartshinder.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace Luftfartshinder.Services
{
    /// <summary>
    /// Service implementation for user-related business logic.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// Validates if email and username are available.
        /// </summary>
        public async Task<(bool IsValid, List<string> Errors)> ValidateUserAsync(string email, string username)
        {
            var errors = new List<string>();

            var existingUserByEmail = await userManager.FindByEmailAsync(email);
            if (existingUserByEmail != null)
            {
                errors.Add("Email is already taken");
            }

            var existingUserByUsername = await userManager.FindByNameAsync(username);
            if (existingUserByUsername != null)
            {
                errors.Add("Username is already taken");
            }

            return (errors.Count == 0, errors);
        }

        /// <summary>
        /// Creates a user with password and assigns a role.
        /// </summary>
        public async Task<(bool Success, IdentityResult? Result)> CreateUserAsync(ApplicationUser user, string password, string role)
        {
            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                return (false, createResult);
            }

            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                return (false, roleResult);
            }

            return (true, null);
        }
    }
}

