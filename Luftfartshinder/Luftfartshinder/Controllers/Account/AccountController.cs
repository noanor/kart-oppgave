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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOrganizationService _organizationService;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of <see cref="AccountController"/>.
        /// </summary>
        /// <param name="userManager">User manager for user operations.</param>
        /// <param name="signInManager">Sign-in manager for authentication.</param>
        /// <param name="accountRepository">Repository for account operations (unused, kept for compatibility).</param>
        /// <param name="reportRepository">Repository for report operations (unused, kept for compatibility).</param>
        /// <param name="organizationRepository">Repository for organization operations (unused, kept for compatibility).</param>
        /// <param name="obstacleRepository">Repository for obstacle operations (unused, kept for compatibility).</param>
        /// <param name="organizationService">Service for organization-related operations.</param>
        /// <param name="userService">Service for user-related operations.</param>
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
            _userManager = userManager;
            _signInManager = signInManager;
            _organizationService = organizationService;
            _userService = userService;
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

            try
            {
                var (isValid, errors) = await _userService.ValidateUserAsync(model.Email, model.Username);
                if (!isValid)
                {
                    AddValidationErrorsToModelState(errors);
                    return View(model);
                }

                var organization = await _organizationService.GetOrCreateOrganizationForRoleAsync(
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

                var (success, result) = await _userService.CreateUserAsync(newUser, model.Password, model.SelectedRole);

                if (success)
                {
                    TempData["Success"] = "User registered successfully!";
                    return RedirectToAction("Dashboard", "Dashboard");
                }

                AddIdentityErrorsToModelState(result);
                return View(model);
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError("", "Unable to register user. Please check your data and try again.");
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while registering the user. Please try again.");
                return View(model);
            }
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

            try
            {
                var (isValid, errors) = await _userService.ValidateUserAsync(model.Email, model.Username);
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

                var organization = await _organizationService.GetOrCreateOrganizationForRoleAsync(
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

                var (success, result) = await _userService.CreateUserAsync(newUser, model.Password, model.SelectedRole);

                if (success)
                {
                    return RedirectToAction("RegistrationPending", "Account");
                }

                AddIdentityErrorsToModelState(result);
                return View(model);
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError("", "Unable to register user. Please check your data and try again.");
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while registering the user. Please try again.");
                return View(model);
            }
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
            ViewData["BodyClass"] = "page-login";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null && !user.IsApproved)
                {
                    ModelState.AddModelError("", "Your account is pending approval by an administrator.");
                    return View(model);
                }

                var signInResult = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
                if (!signInResult.Succeeded)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }

                return await RedirectToDashboardByRoleAsync(user);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        /// <summary>
        /// Handles user sign out.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public new async Task<IActionResult> SignOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
                TempData["Success"] = "You have been signed out successfully";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred during sign out.";
            }
            
            return RedirectToAction("Login", "Account");
        }        
    }
}
