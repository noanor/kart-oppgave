using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Shared;
using Luftfartshinder.Models.ViewModel.User;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IOrganizationRepository organizationRepository;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IReportRepository reportRepository, IOrganizationRepository organizationRepository, IObstacleRepository obstacleRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.organizationRepository = organizationRepository;
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
            var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
            var existingUserByUsername = await userManager.FindByNameAsync(model.Username);

            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("Email", "Email is already taken");
                return View(model);
            }

            if (existingUserByUsername != null)
            {
                ModelState.AddModelError("Username", "Username is already taken");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? organizationName = null;
            if (model.SelectedRole == "FlightCrew")
            {
                if (model.OrganizationName == "Other" && !string.IsNullOrEmpty(model.OtherOrganizationName))
                    organizationName = model.OtherOrganizationName;
                else
                    organizationName = model.OrganizationName;
            }

            var organization = await organizationRepository.GetByName(organizationName);

            var newUser = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username,
                Organization = organization,
                IsApproved = true
            };

            var createResult = await userManager.CreateAsync(newUser, model.Password);

            if (createResult.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(newUser, model.SelectedRole);

                if (roleResult.Succeeded)
                {
                    TempData["RegistrationSuccess"] = "User registered successfully!";
                    return RedirectToAction("Dashboard", "Dashboard");
                }
            }

            return RedirectToAction("List", "SuperadminHome");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult UserRegister()
        {
            return View(new UserRegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UserRegister(RegisterViewModel model)
        {
            var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
            var existingUserByUsername = await userManager.FindByNameAsync(model.Username);

            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("Email", "Email is already taken");
                return View(model);
            }

            if (existingUserByUsername != null)
            {
                ModelState.AddModelError("Username", "Username is already taken");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Organization? organization = null;

            if (model.SelectedRole == "FlightCrew")
            {
                string? orgName = null;

                if (model.OrganizationName == "Other" &&
                    !string.IsNullOrWhiteSpace(model.OtherOrganizationName))
                {
                    orgName = model.OtherOrganizationName;
                }
                else
                {
                    orgName = model.OrganizationName;
                }

                if (!string.IsNullOrWhiteSpace(orgName))
                {
                    // look up existing org, or create new
                    // depends on how you store organizations
                    organization = await organizationRepository.GetByName(orgName);

                    if (organization == null)
                    {
                        organization = new Organization { Name = orgName };
                        await organizationRepository.Add(organization);
                    }
                }
            }

            var newUser = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username,
                OrganizationId = organization.Id, // or Organization = organization if same DbContext
                IsApproved = true
            };

            var createResult = await userManager.CreateAsync(newUser, model.Password);

            if (createResult.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(newUser, model.SelectedRole);

                if (roleResult.Succeeded)
                {
                    TempData["RegistrationSuccess"] = "User registered successfully!";
                    return RedirectToAction("Dashboard", "Dashboard");
                }
            }

            // Add any errors from createResult / roleResult to ModelState if you want
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            //if (User.Identity.IsAuthenticated && User.IsInRole("Registrar"))
            //{
            //    return RedirectToAction("Dashboard", "Dashboard");
            //}
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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
                return RedirectToAction("Dashboard", "Dashboard");

            if (roles.Contains("Registrar"))
                return RedirectToAction("Dashboard", "Dashboard");

            if (roles.Contains("FlightCrew"))
                return RedirectToAction("Index", "Home");

            return RedirectToAction("Tutorial", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();

            TempData["SignOutMessage"] = "You have been signed out successfully";

            return RedirectToAction("Login", "Account");
        }

        

    }
}
