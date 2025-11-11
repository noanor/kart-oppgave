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

        public async Task<IActionResult> List(string roleFilter = "All")
        {
            var allUsers = await userRepository.GetAll();
            var filteredUsers = new List<User>();

            foreach (var user in allUsers)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var userRole = userRoles.FirstOrDefault() ?? "No Role";
                
                if (roleFilter != "All" && userRole != roleFilter)
                {
                    continue;
                }
                
                filteredUsers.Add(new User
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName,
                    EmailAdress = user.Email,
                    IsApproved = user.IsApproved,
                    Role = userRole
                });
            }
            
            var viewModel = new UserViewModel { Users = filteredUsers };
            ViewBag.CurrentFilter = roleFilter;
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
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

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id)
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

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(Guid id)
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

            return RedirectToAction("List");
        }
    }
}
