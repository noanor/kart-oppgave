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
        [Authorize(Roles = "Superadmin")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Superadmin")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var nyBruker = new ApplicationUser()
            {
                Fornavn = model.Fornavn,
                Etternavn = model.Etternavn,
                Email = model.Email,
                UserName = model.Brukernavn 
            };

            var nybrukerResultat = await userManager.CreateAsync(nyBruker, model.Passord);

            if (nybrukerResultat.Succeeded)
            {
                var tildelingRolleResultat = await userManager.AddToRoleAsync(nyBruker, model.Rolle);

                if (tildelingRolleResultat.Succeeded)
                {
                    return RedirectToAction("Register");
                }
            }
            
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var loginnResultat = await signInManager.PasswordSignInAsync(model.Brukernavn, model.Passord, false, false);

            if (loginnResultat.Succeeded)
            {
                var bruker = await userManager.FindByNameAsync(model.Brukernavn);
                var roller = await userManager.GetRolesAsync(bruker);
                
                if (roller.Contains("Superadmin"))
                    return RedirectToAction("SuperAdminHome", "Home");
                else if (roller.Contains("Flybesetning"))
                    return RedirectToAction("Index", "Home");
                else if (roller.Contains("Registerforer"))
                    return RedirectToAction("RegisterforerHome", "Home");
            }
            
            ModelState.AddModelError(string.Empty, "Feil brukernavn eller passord");
            return View();
        }
        
    }
}