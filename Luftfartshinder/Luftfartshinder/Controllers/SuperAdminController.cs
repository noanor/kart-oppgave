using Luftfartshinder.Models;
using Luftfartshinder.Models.ViewModel;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public SuperAdminController(IUserRepository userRepository, UserManager<ApplicationUser> userManager)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        public async Task<IActionResult> List(string roleFilter = "All", string statusFilter = "", string organizationFilter = "")
        {
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
                
                var knownOrgs = new List<string> { "Police", "Norwegian Air Ambulance", "Avinor", "Norwegian Armed Forces" };

                if (!string.IsNullOrEmpty(organizationFilter))
                {
                    if (organizationFilter == "Other")
                    {
                        if (knownOrgs.Contains(user.Organization))
                        {
                            continue; 
                        }
                    }
                    else if (user.Organization != organizationFilter)
                    {
                        continue;
                    }
                }
                
                if (!string.IsNullOrEmpty(user.Organization))
                {
                    uniqueOrganizations.Add(user.Organization);
                }
                
                filteredUsers.Add(new User
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName,
                    EmailAdress = user.Email,
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
