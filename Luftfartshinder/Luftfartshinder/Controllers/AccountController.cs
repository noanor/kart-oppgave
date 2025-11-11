using Luftfartshinder.Models;
using Luftfartshinder.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var newUser = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username,
                IsApproved = true
            };

            var createResult = await userManager.CreateAsync(newUser, model.Password);

            if (createResult.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(newUser, model.SelectedRole);

                if (roleResult.Succeeded)
                {
                    return RedirectToAction("SuperAdminHome", "Home"); 
                }
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult UserRegister()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UserRegister(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var newUser = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username
            };

            var createResult = await userManager.CreateAsync(newUser, model.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var roleResult = await userManager.AddToRoleAsync(newUser, model.SelectedRole);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            TempData["RegistrationSuccess"] = "Registration successful! Please wait for admin approval before logging in.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            
            if (user != null && !user.IsApproved)
            {
                ModelState.AddModelError("", "Your account is pending approval by an administrator.");
                return View();
            }

            var signInResult = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var roles = await userManager.GetRolesAsync(user);

            if (roles.Contains("SuperAdmin"))
                return RedirectToAction("SuperAdminHome", "Home");

            if (roles.Contains("Registrar"))
                return RedirectToAction("RegistrarHome", "Home");

            if (roles.Contains("FlightCrew"))
                return RedirectToAction("Index", "Home");
                
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            
            TempData["SignOutMessage"] = "Goodbye! You have been logged out successfully.";
            
            return RedirectToAction("Login", "Account");
        }
    }
}
