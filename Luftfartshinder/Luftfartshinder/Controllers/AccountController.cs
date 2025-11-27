using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Organization;
using Luftfartshinder.Models.ViewModel.Shared;
using Luftfartshinder.Models.ViewModel.User;
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
        private readonly IOrganizationRepository organizationRepository;
        private readonly IObstacleRepository obstacleRepository;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAccountRepository accountRepository, IReportRepository reportRepository, IOrganizationRepository organizationRepository, IObstacleRepository obstacleRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.accountRepository = accountRepository;
            this.reportRepository = reportRepository;
            this.organizationRepository = organizationRepository;
            this.obstacleRepository = obstacleRepository;
        }

        // Admin brukerregistrering: PC-vennlig layout
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Register()
        {
            ViewData["LayoutType"] = "pc";
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

            Organization? organization = null;

            if (model.SelectedRole == "FlightCrew")
            {
                string? organizationName = null;
                if (model.OrganizationName == "Other" && !string.IsNullOrEmpty(model.OtherOrganizationName))
                    organizationName = model.OtherOrganizationName;
                else
                    organizationName = model.OrganizationName;

                if (!string.IsNullOrWhiteSpace(organizationName))
                {
                    organization = await organizationRepository.GetByName(organizationName);
                }
            }
            else if (model.SelectedRole == "Registrar")
            {
                // Registrar brukere tilhører Kartverket organisasjonen
                organization = await organizationRepository.GetByName("Kartverket");

                if (organization == null)
                {
                    // Hvis Kartverket ikke finnes, opprett den
                    organization = new Organization { Name = "Kartverket" };
                    await organizationRepository.Add(organization);
                }
            }

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

            var createResult = await userManager.CreateAsync(newUser, model.Password);

            if (createResult.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(newUser, model.SelectedRole);

                if (roleResult.Succeeded)
                {
                    TempData["RegistrationSuccess"] = "User registered successfully!";
                    return RedirectToAction("Dashboard", "Account");
                }
            }

            return RedirectToAction("List", "SuperadminHome");
        }

        // Brukerregistrering: iPad-vennlig layout
        [HttpGet]
        [AllowAnonymous]
        public IActionResult UserRegister()
        {
            ViewData["LayoutType"] = "ipad";
            return View(new UserRegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UserRegister(UserRegisterViewModel model)
        {
            ViewData["LayoutType"] = "ipad";

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

                if (string.IsNullOrWhiteSpace(orgName))
                {
                    ModelState.AddModelError("OrganizationName", "Organization is required for FlightCrew role.");
                    return View(model);
                }

                // look up existing org, or create new
                organization = await organizationRepository.GetByName(orgName);

                if (organization == null)
                {
                    organization = new Organization { Name = orgName };
                    await organizationRepository.Add(organization);
                }
            }
            else if (model.SelectedRole == "Registrar")
            {
                // Registrar brukere tilhører Kartverket organisasjonen
                organization = await organizationRepository.GetByName("Kartverket");

                if (organization == null)
                {
                    // Hvis Kartverket ikke finnes, opprett den
                    organization = new Organization { Name = "Kartverket" };
                    await organizationRepository.Add(organization);
                }
            }

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

            var createResult = await userManager.CreateAsync(newUser, model.Password);

            if (createResult.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(newUser, model.SelectedRole);

                if (roleResult.Succeeded)
                {
                    return RedirectToAction("RegistrationPending", "Account");
                }
                else
                {
                    // Add role assignment errors to ModelState
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                // Add user creation errors to ModelState
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // Registrering venter på godkjenning
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegistrationPending()
        {
            ViewData["LayoutType"] = "ipad";
            return View();
        }


        // Login-side: iPad-vennlig layout
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Registrar"))
            {
                return RedirectToAction("Dashboard");
            }
            ViewData["LayoutType"] = "ipad";
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

            TempData["SignOutMessage"] = "You have been signed out successfully";

            return RedirectToAction("Login", "Account");
        }

        // Dashboard: iPad-vennlig layout for FlightCrew, PC-vennlig for admin/registrar
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            // FlightCrew (pilot) skal ha iPad-navbar på alle sider
            if (User.IsInRole("FlightCrew") && !User.IsInRole("SuperAdmin"))
            {
                ViewData["LayoutType"] = "ipad";
            }
            else
            {
                ViewData["LayoutType"] = "pc";
            }

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

        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> FlightCrewObstacles()
        //{
        //    string? userId = userManager.GetUserId(User);
        //    var obstacles = new List<Report>();

        //    obstacles = await reportRepository.GetAllAsync();

        //    return View(obstacles);
        //}

        // FlightCrew obstacles: iPad-vennlig layout for FlightCrew
        public async Task<IActionResult> FlightCrewObstacles()
        {
            // FlightCrew (pilot) skal ha iPad-navbar på alle sider
            ViewData["LayoutType"] = "ipad";
            // 1. Finn innlogget bruker
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                // Ingen innlogget bruker → redirect eller forbidd, opp til deg
                return Challenge(); // sender til login
            }

            if (user.OrganizationId == null)
            {
                // Brukeren har ikke organisasjon
                return Forbid(); // eller vis en egen side
            }

            var organization = await organizationRepository.GetById(user.OrganizationId);

            var orgId = organization.Id;

            // 2. Hent data fra Domain-DB via repos
            var obstacles = await obstacleRepository.GetByOrgId(orgId);
            var reports = await reportRepository.GetByOrgId(orgId);

            // 3. Bygg viewmodel
            var vm = new OrgDataViewModel
            {
                Organization = organization,
                Obstacles = obstacles,
                Reports = reports
            };

            // 4. Send til view
            return View("FlightCrewObstacles", vm);
        }

    }
}
