using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Organization;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers
{
    /// <summary>
    /// Controller for dashboard views and organization-related obstacle/report data.
    /// Provides controlled access based on user and organization identity.
    /// </summary>
    public class DashboardController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IObstacleRepository _obstacleRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of <see cref="DashboardController"/>.
        /// </summary>
        /// <param name="reportRepository">Repository for retrieving reports.</param>
        /// <param name="obstacleRepository">Repository for retrieving obstacles.</param>
        /// <param name="organizationRepository">Repository for retrieving organizations.</param>
        /// <param name="userManager">User manager for accessing authenticated user data.</param>
        public DashboardController(
            IReportRepository reportRepository,
            IObstacleRepository obstacleRepository,
            IOrganizationRepository organizationRepository,
            UserManager<ApplicationUser> userManager)
        {
            _reportRepository = reportRepository;
            _obstacleRepository = obstacleRepository;
            _organizationRepository = organizationRepository;
            _userManager = userManager;
        }
        
        /// <summary>
        /// Displays the dashboard view showing all reports.
        /// Accessible to Registrar and SuperAdmin roles. They can see all obstacles and reports.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Registrar, SuperAdmin")]
        public async Task<IActionResult> Dashboard()
        {
            var reports = await _reportRepository.GetAllAsync();
            return View("~/Views/Account/Dashboard.cshtml", reports);
        }

        /// <summary>
        /// Displays obstacles and reports associated with the logged-in user's organization.
        /// Accessible to FlightCrew, Registrar and SuperAdmin. FlightCrew users see only their organization's data.
        /// </summary>
        [Authorize(Roles = "FlightCrew, Registrar, SuperAdmin")]
        public async Task<IActionResult> FlightCrewObstacles()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            if (user.OrganizationId == null || user.OrganizationId == 0)
            {
                return Forbid();
            }

            var organization = await _organizationRepository.GetById(user.OrganizationId);

            if (organization == null)
            {
                return NotFound("The organization linked to your account could not be found.");
            }

            var orgId = organization.Id;

            var obstacles = await _obstacleRepository.GetByOrgId(orgId);
            var reports = await _reportRepository.GetByOrgId(orgId);

            var viewModel = new OrgDataViewModel
            {
                Organization = organization,
                Obstacles = obstacles,
                Reports = reports
            };

            return View("FlightCrewObstacles", viewModel);
        }
    }
}