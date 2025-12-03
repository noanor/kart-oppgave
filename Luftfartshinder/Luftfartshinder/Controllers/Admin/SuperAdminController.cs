using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.User;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers.Admin
{
    /// <summary>
    /// Controller for SuperAdmin functionality including user management.
    /// </summary>
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController(IUserRepository userRepository, UserManager<ApplicationUser> userManager, IOrganizationRepository organizationRepository) : Controller
    {
        private readonly IUserRepository userRepository = userRepository;
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly IOrganizationRepository organizationRepository = organizationRepository;

        /// <summary>
        /// Displays a filtered list of users with role, status, and organization filters.
        /// </summary>
        public async Task<IActionResult> List(string roleFilter = "All", string statusFilter = "", string organizationFilter = "")
        { 
            ViewData["LayoutType"] = "pc";
            var allUsers = await userRepository.GetAll();
            var filteredUsers = new List<User>();
            var uniqueOrganizations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    
            var knownOrgNames = new[] {
                "Avinor",
                "Norwegian Air Ambulance",
                "Norwegian Armed Forces",
                "Police"
            };

            foreach (var user in allUsers)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var userRole = userRoles.FirstOrDefault() ?? "No Role";
        
                if (roleFilter != "All" && userRole != roleFilter)
                {
                    continue;
                }
        
                if (!string.IsNullOrEmpty(statusFilter))
                {   
                    bool isApproved = statusFilter == "Approved";
                    if (user.IsApproved != isApproved)
                    {
                        continue;
                    }
                }
        
                if (!string.IsNullOrEmpty(organizationFilter))
                {
                    var userOrgName = user.Organization?.Name?.Trim();

                    if (organizationFilter == "Other")
                    {
                        if (!string.IsNullOrEmpty(userOrgName)
                            && knownOrgNames.Contains(userOrgName, StringComparer.OrdinalIgnoreCase))
                        {
                            continue; 
                        }
                    }
                    else
                    {
                        if (!string.Equals(userOrgName, organizationFilter?.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(user.Organization?.Name))
                {
                    uniqueOrganizations.Add(user.Organization.Name.Trim());
                }
                
                filteredUsers.Add(new User
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName ?? string.Empty,
                    EmailAddress = user.Email ?? string.Empty,
                    IsApproved = user.IsApproved,
                    Role = userRole,
                    Organization = user.Organization
                });
            }

            var viewModel = new UserViewModel { Users = filteredUsers };
            ViewBag.CurrentFilter = roleFilter;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.OrganizationFilter = organizationFilter;
            ViewBag.Organizations = uniqueOrganizations.OrderBy(o => o).ToList();
            ViewBag.TotalUsers = allUsers.Count();
            
            if (!filteredUsers.Any())
            {
                var parts = new List<string>();

                if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "All")
                {
                    parts.Add($"Role: {roleFilter}");
                }

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    parts.Add($"Status: {statusFilter}");
                }

                if (!string.IsNullOrEmpty(organizationFilter))
                {
                    parts.Add($"Organization: {organizationFilter}");
                }

                string message;
                if (parts.Count == 0)
                {
                    message = "No users found.";
                }
                else
                {
                    message = "No users found matching " + string.Join(" and ", parts) + ".";
                }

                ViewBag.NoUsersMessage = message;
            }
            else
            {
                ViewBag.NoUsersMessage = null;
            }
            
            return View(viewModel);
        }
        
        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, string roleFilter, string statusFilter, string organizationFilter)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                var username = user.UserName;
                var deleteResult = await userManager.DeleteAsync(user);

                if (deleteResult.Succeeded)
                {
                    TempData["Success"] = $"User '{username}' has been successfully deleted.";
                }
            }

            return RedirectToAction("List", new { roleFilter, statusFilter, organizationFilter });
        }

        /// <summary>
        /// Approves a user, allowing them to log in.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id, string roleFilter, string statusFilter, string organizationFilter)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                user.IsApproved = true;
                var updateResult = await userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    TempData["Success"] = $"User '{user.UserName}' has been approved and can now log in.";
                }
            }

            return RedirectToAction("List", new { roleFilter, statusFilter, organizationFilter });
        }

        /// <summary>
        /// Declines a user registration and removes them from the system.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(Guid id, string roleFilter, string statusFilter, string organizationFilter)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                var username = user.UserName;
                var deleteResult = await userManager.DeleteAsync(user);

                if (deleteResult.Succeeded)
                {
                    TempData["Success"] = $"User '{username}' has been declined and removed from the system.";
                }
            }

            return RedirectToAction("List", new { roleFilter, statusFilter, organizationFilter });
        }
    }
}
