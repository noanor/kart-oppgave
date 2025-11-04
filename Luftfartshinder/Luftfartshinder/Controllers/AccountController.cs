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
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            var identityUser = new ApplicationUser
            {
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                Email = registerViewModel.Email,
                UserName = registerViewModel.Username
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerViewModel.Password);

            if (identityResult.Succeeded)
            {
                //Nå gir man brukeren er rolle
                var roleIdentityResult = await userManager.AddToRoleAsync(identityUser, registerViewModel.SelectedRole);

                if (roleIdentityResult.Succeeded)
                {
                    //Hvis notifikasjon sucess
                    return RedirectToAction("Register"); 
                }


            }

            // Vis feil notifikasjon
            return View();

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            var signInResult = await signInManager.PasswordSignInAsync(loginViewModel.Username, loginViewModel.Password, false, false);

            if (signInResult.Succeeded)
            {
                var user = await userManager.FindByNameAsync(loginViewModel.Username);
                var roles = await userManager.GetRolesAsync(user);

                if (roles.Contains("SuperAdmin"))
                    return RedirectToAction("SuperAdminHome", "Home");

                if (roles.Contains("Registrar"))
                    return RedirectToAction("RegistrarHome", "Home");

                if (roles.Contains("FlightCrew"))
                    return RedirectToAction("Index", "Home");
                
                return RedirectToAction("index", "Home");
            }
            //viser feil
            ModelState.AddModelError("", "Invalid username or password");
            return View();
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
