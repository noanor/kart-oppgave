using Luftfartshinder.Models;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Luftfartshinder.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IAccountRepository accountRepository;
        private readonly IReportRepository reportRepository;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAccountRepository accountRepository, IReportRepository reportRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.accountRepository = accountRepository;
            this.reportRepository = reportRepository;
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

            string? organization = null;
            if (model.SelectedRole == "FlightCrew")
            {
                if (model.Organization == "Other" && !string.IsNullOrEmpty(model.OtherOrganization))
                    organization = model.OtherOrganization;
                else
                    organization = model.Organization;
            }

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
                    return RedirectToAction("Dashboard");
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

            string? organization = null;
            if (model.SelectedRole == "FlightCrew")
            {
                if (model.Organization == "Other" && !string.IsNullOrEmpty(model.OtherOrganization))
                    organization = model.OtherOrganization;
                else
                    organization = model.Organization;
            }

            var newUser = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username,
                Organization = organization,
                IsApproved = false
            };

            var createResult = await userManager.CreateAsync(newUser, model.Password);

            if (!createResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Registration failed. Please correct the fields below.");

                foreach (var error in createResult.Errors)
                {
                    if (error.Code.Contains("DuplicateUserName"))
                        ModelState.AddModelError("Username", "This username is already taken.");
                    else if (error.Code.Contains("DuplicateEmail"))
                        ModelState.AddModelError("Email", "This email is already taken.");
                    else
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
            if (User.Identity.IsAuthenticated && User.IsInRole("Registrar"))
            {
                return RedirectToAction("Dashboard");
            }
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
                return RedirectToAction("Dashboard");

            if (roles.Contains("Registrar"))
                return RedirectToAction("Dashboard");

            if (roles.Contains("FlightCrew"))
                return RedirectToAction("Index", "Home");

            return RedirectToAction("Tutorial", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();

            TempData["SignOutMessage"] = "Goodbye! You have been logged out successfully.";

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reports = new List<Report>();
            if (!User.IsInRole("Registrar") && !User.IsInRole("SuperAdmin"))
            {
                reports = accountRepository.GetUserReports(userId);
            }
            else
            {
                reports = await reportRepository.GetAllAsync();
            }
            return View(reports);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> FlightCrewObstacles()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var obstacles = new List<Report>();

            obstacles = await reportRepository.GetAllAsync();

            return View(obstacles);
        }


    }
}
