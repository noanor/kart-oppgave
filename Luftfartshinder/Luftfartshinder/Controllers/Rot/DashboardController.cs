using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Organization;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Luftfartshinder.Models.ViewModel.Shared;
using System.Diagnostics;

namespace Luftfartshinder.Controllers.Rot
{
    public class DashboardController : Controller
    {
        private readonly IReportRepository reportRepository;
        private readonly IObstacleRepository obstacleRepository;
        private readonly IOrganizationRepository organizationRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public DashboardController(IReportRepository reportRepository, IObstacleRepository obstacleRepository, IOrganizationRepository organizationRepository)
        {
            this.reportRepository = reportRepository;
            this.obstacleRepository = obstacleRepository;
            this.organizationRepository = organizationRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {

            var reports = await reportRepository.GetAllAsync();

            return View("~/Views/Account/Dashboard.cshtml", reports);
        }

        public async Task<IActionResult> FlightCrewObstacles()
        {
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
