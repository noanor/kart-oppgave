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
        private readonly IUserRepository _userRepository = userRepository;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IOrganizationRepository _organizationRepository = organizationRepository;

        /// <summary>
        /// Displays a filtered list of users with role, status, and organization filters.
        /// </summary>
        /// <param name="roleFilter">Filter by user role.</param>
        /// <param name="statusFilter">Filter by approval status.</param>
        /// <param name="organizationFilter">Filter by organization name.</param>
        public async Task<IActionResult> List(string roleFilter = "All", string statusFilter = "", string organizationFilter = "")
        { 
            ViewData["LayoutType"] = "pc";
            
            try
            {
                var allUsers = await _userRepository.GetAll();
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
                var userRoles = await _userManager.GetRolesAsync(user);
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
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while loading users. Please try again.";
                return View(new UserViewModel { Users = new List<User>() });
            }
        }
        
        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <param name="roleFilter">Current role filter to preserve after redirect.</param>
        /// <param name="statusFilter">Current status filter to preserve after redirect.</param>
        /// <param name="organizationFilter">Current organization filter to preserve after redirect.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, string roleFilter, string statusFilter, string organizationFilter)
        {
            if (id == Guid.Empty)
            {
                TempData["Error"] = "Invalid user ID.";
                return RedirectToAction("List", new { roleFilter, statusFilter, organizationFilter });
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("List", new { roleFilter, statusFilter, organizationFilter });
                }

                var username = user.UserName;
                var deleteResult = await _userManager.DeleteAsync(user);

                if (deleteResult.Succeeded)
                {
                    TempData["Success"] = $"User '{username}' has been successfully deleted.";
                }
                else
                {
                    var errors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
                    TempData["Error"] = $"Unable to delete user: {errors}";
                }
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Unable to delete the user. Please try again.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred while deleting the user. Please try again.";
            }

            return RedirectToAction("List", new { roleFilter, statusFilter, organizationFilter });
        }

        /// <summary>
        /// Approves a user, allowing them to log in.
        /// </summary>
        /// <param name="id">The ID of the user to approve.</param>
        /// <param name="roleFilter">Current role filter to preserve after redirect.</param>
        /// <param name="statusFilter">Current status filter to preserve after redirect.</param>
        /// <param name="organizationFilter">Current organization filter to preserve after redirect.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id, string roleFilter, string statusFilter, string organizationFilter)
        {
            if (id == Guid.Empty)
            {
                TempData["Error"] = "Invalid user ID.";
                return RedirectToAction("List", new { roleFilter, statusFilter, organizationFilter });
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("List", new { roleFilter, statusFilter, organizationFilter });
                }

                user.IsApproved = true;
                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    TempData["Success"] = $"User '{user.UserName}' has been approved and can now log in.";
                }
                else
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    TempData["Error"] = $"Unable to approve user: {errors}";
                }
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Unable to approve the user. Please try again.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred while approving the user. Please try again.";
            }

            return RedirectToAction("List", new { roleFilter, statusFilter, organizationFilter });
        }

        /// <summary>
        /// Declines a user registration and removes them from the system.
        /// </summary>
        /// <param name="id">The ID of the user to decline.</param>
        /// <param name="roleFilter">Current role filter to preserve after redirect.</param>
        /// <param name="statusFilter">Current status filter to preserve after redirect.</param>
        /// <param name="organizationFilter">Current organization filter to preserve after redirect.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(Guid id, string roleFilter, string statusFilter, string organizationFilter)
        {
            if (id == Guid.Empty)
            {
                TempData["Error"] = "Invalid user ID.";
                return RedirectToAction("List", new { roleFilter, statusFilter, organizationFilter });
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("List", new { roleFilter, statusFilter, organizationFilter });
                }

                var username = user.UserName;
                var deleteResult = await _userManager.DeleteAsync(user);

                if (deleteResult.Succeeded)
                {
                    TempData["Success"] = $"User '{username}' has been declined and removed from the system.";
                }
                else
                {
                    var errors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
                    TempData["Error"] = $"Unable to decline user: {errors}";
                }
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Unable to decline the user. Please try again.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred while declining the user. Please try again.";
            }

            return RedirectToAction("List", new { roleFilter, statusFilter, organizationFilter });
        }
    }
}
