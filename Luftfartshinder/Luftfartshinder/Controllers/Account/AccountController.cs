using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Shared;
using Luftfartshinder.Models.ViewModel.User;
using Luftfartshinder.Repository;
using Luftfartshinder.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers.Account
{
    /// <summary>
    /// Controller for handling user authentication, registration, and account management.
    /// </summary>
    public partial class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IOrganizationService organizationService;
        private readonly IUserService userService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IAccountRepository accountRepository,
            IReportRepository reportRepository,
            IOrganizationRepository organizationRepository,
            IObstacleRepository obstacleRepository,
            IOrganizationService organizationService,
            IUserService userService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.organizationService = organizationService;
            this.userService = userService;
        }

        /// <summary>
        /// Displays the admin user registration form.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Register()
        {
            ViewData["LayoutType"] = "pc";
            return View();
        }

        /// <summary>
        /// Handles admin user registration.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewData["LayoutType"] = "pc";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (isValid, errors) = await userService.ValidateUserAsync(model.Email, model.Username);
            if (!isValid)
            {
                AddValidationErrorsToModelState(errors);
                return View(model);
            }

            var organization = await organizationService.GetOrCreateOrganizationForRoleAsync(
                model.SelectedRole,
                model.OrganizationName,
                model.OtherOrganizationName);

            if (organization == null)
            {
                ModelState.AddModelError("", "Unable to set organization. Please try again.");
                return View(model);
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

            var (success, result) = await userService.CreateUserAsync(newUser, model.Password, model.SelectedRole);

            if (success)
            {
                TempData["RegistrationSuccess"] = "User registered successfully!";
                return RedirectToAction("Dashboard", "Dashboard");
            }

            AddIdentityErrorsToModelState(result);
            return View(model);
        }

        /// <summary>
        /// Displays the public user registration form.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult UserRegister()
        {
            ViewData["LayoutType"] = "ipad";
            return View(CreateEmptyUserRegisterViewModel());
        }

        /// <summary>
        /// Handles public user registration.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> UserRegister(UserRegisterViewModel model)
        {
            ViewData["LayoutType"] = "ipad";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (isValid, errors) = await userService.ValidateUserAsync(model.Email, model.Username);
            if (!isValid)
            {
                AddValidationErrorsToModelState(errors);
                return View(model);
            }

            if (model.SelectedRole == "FlightCrew" && 
                string.IsNullOrWhiteSpace(model.OrganizationName) && 
                string.IsNullOrWhiteSpace(model.OtherOrganizationName))
            {
                ModelState.AddModelError("OrganizationName", "Organization is required for FlightCrew role.");
                return View(model);
            }

            var organization = await organizationService.GetOrCreateOrganizationForRoleAsync(
                model.SelectedRole,
                model.OrganizationName,
                model.OtherOrganizationName);

            if (organization == null)
            {
                ModelState.AddModelError("", "Unable to set organization. Please try again.");
                return View(model);
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

            var (success, result) = await userService.CreateUserAsync(newUser, model.Password, model.SelectedRole);

            if (success)
            {
                return RedirectToAction("RegistrationPending", "Account");
            }

            AddIdentityErrorsToModelState(result);
            return View(model);
        }

        /// <summary>
        /// Displays the registration pending confirmation page.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegistrationPending()
        {
            ViewData["LayoutType"] = "ipad";
            return View();
        }

        /// <summary>
        /// Displays the login form.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Registrar"))
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }
            ViewData["LayoutType"] = "ipad";
            ViewData["BodyClass"] = "page-login";
            return View();
        }

        /// <summary>
        /// Handles user login authentication.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["LayoutType"] = "ipad";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && !user.IsApproved)
            {
                ModelState.AddModelError("", "Your account is pending approval by an administrator.");
                return View(model);
            }

            var signInResult = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            return await RedirectToDashboardByRoleAsync(user);
        }

        /// <summary>
        /// Handles user sign out.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public new async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            TempData["SignOutMessage"] = "You have been signed out successfully";
            return RedirectToAction("Login", "Account");
        }        
    }
}
