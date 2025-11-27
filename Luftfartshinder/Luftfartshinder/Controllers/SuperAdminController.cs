using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.User;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController(IUserRepository userRepository, UserManager<ApplicationUser> userManager, IOrganizationRepository organizationRepository) : Controller
    {
        private readonly IUserRepository userRepository = userRepository;
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly IOrganizationRepository organizationRepository = organizationRepository;

        // Brukerliste: PC-vennlig layout (tabell-visning)
        public async Task<IActionResult> List(string roleFilter = "All", string statusFilter = "", string organizationFilter = "")
        {
            ViewData["LayoutType"] = "pc";
            var allUsers = await userRepository.GetAll();
            var filteredUsers = new List<User>();
            var uniqueOrganizations = new HashSet<string>();

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

                var knownOrgs = organizationRepository.GetAll();

                if (!string.IsNullOrEmpty(organizationFilter))
                {
                    if (organizationFilter == "Other")
                    {
                        foreach (var o in knownOrgs)
                        {
                            if (o.Name == organizationFilter)
                            {
                                continue;
                            }
                        }
                    }
                    else if (user.Organization.Name != organizationFilter)
                    {
                        continue;
                    }
                }
                
                if (!string.IsNullOrEmpty(user.Organization.Name))
                {
                    var newUniqueOrganization = new Organization
                    {
                        Name = user.Organization.Name

                    };
                    uniqueOrganizations.Add(user.Organization.Name);
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
            
            return View(viewModel);
        }

        [HttpPost]
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

        [HttpPost]
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

        [HttpPost]
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
