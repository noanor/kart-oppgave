using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Organization;
using Luftfartshinder.Models.ViewModel.Shared;
using Luftfartshinder.Models.ViewModel.User;
using Luftfartshinder.Repository;
using Luftfartshinder.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Luftfartshinder.Controllers
{
    /// <summary>
    /// Controller for handling user authentication, registration, and account management.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IAccountRepository accountRepository;
        private readonly IReportRepository reportRepository;
        private readonly IOrganizationRepository organizationRepository;
        private readonly IObstacleRepository obstacleRepository;
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
            this.accountRepository = accountRepository;
            this.reportRepository = reportRepository;
            this.organizationRepository = organizationRepository;
            this.obstacleRepository = obstacleRepository;
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
                return RedirectToAction("Dashboard", "Account");
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
                return RedirectToAction("Dashboard");
            }
            ViewData["LayoutType"] = "ipad";
            return View();
        }

        /// <summary>
        /// Handles user login authentication.
        /// </summary>
        [HttpPost]
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

        /// <summary>
        /// Displays the user dashboard with reports based on user role.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            if (User.IsInRole("FlightCrew") && !User.IsInRole("SuperAdmin"))
            {
                ViewData["LayoutType"] = "ipad";
            }
            else
            {
                ViewData["LayoutType"] = "pc";
            }
            var reports = await GetReportsForUserAsync();
            return View(reports);
        }

        /// <summary>
        /// Displays obstacles and reports for FlightCrew users.
        /// </summary>
        [Authorize]
        public async Task<IActionResult> FlightCrewObstacles()
        {
            ViewData["LayoutType"] = "ipad";

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            if (user.OrganizationId == 0)
            {
                return Forbid();
            }

            var organization = await organizationRepository.GetById(user.OrganizationId);
            if (organization == null)
            {
                return NotFound();
            }

            var obstacles = await obstacleRepository.GetByOrgId(organization.Id);
            var reports = await reportRepository.GetByOrgId(organization.Id);

            var viewModel = new OrgDataViewModel
            {
                Organization = organization,
                Obstacles = obstacles,
                Reports = reports
            };

            return View("FlightCrewObstacles", viewModel);
        }

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
                return RedirectToAction("Dashboard");
            }

            if (roles.Contains("FlightCrew"))
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Tutorial", "Home");
        }

        private async Task<List<Report>> GetReportsForUserAsync()
        {
            if (User.IsInRole("Registrar") || User.IsInRole("SuperAdmin"))
            {
                return await reportRepository.GetAllAsync();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await accountRepository.GetUserReports(userId);
        }
    }
}
