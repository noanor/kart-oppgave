using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers.Account
{
    public partial class AccountController : Controller
    {
        // Private helper methods

        private UserRegisterViewModel CreateEmptyUserRegisterViewModel()
        {
            return new UserRegisterViewModel
            {
                FirstName = string.Empty,
                LastName = string.Empty,
                Email = string.Empty,
                Username = string.Empty,
                Password = string.Empty,
                SelectedRole = string.Empty,
                ConfirmPassword = string.Empty
            };
        }

        private void AddValidationErrorsToModelState(List<string> errors)
        {
            foreach (var error in errors)
            {
                if (error.Contains("Email"))
                {
                    ModelState.AddModelError("Email", error);
                }
                else if (error.Contains("Username"))
                {
                    ModelState.AddModelError("Username", error);
                }
                else
                {
                    ModelState.AddModelError("", error);
                }
            }
        }

        private void AddIdentityErrorsToModelState(IdentityResult? result)
        {
            if (result == null) return;

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        private async Task<IActionResult> RedirectToDashboardByRoleAsync(ApplicationUser? user)
        {
            if (user == null)
            {
                return RedirectToAction("Tutorial", "Home");
            }

            var roles = await userManager.GetRolesAsync(user);

            if (roles.Contains("SuperAdmin") || roles.Contains("Registrar"))
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }

            if (roles.Contains("FlightCrew"))
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Tutorial", "Home");
        }
    }
}
