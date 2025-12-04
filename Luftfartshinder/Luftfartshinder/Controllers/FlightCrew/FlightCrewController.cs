using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.FlightCrew;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Luftfartshinder.Controllers.FlightCrew
{
    [Authorize(Roles = "FlightCrew")]
    public class FlightCrewController : Controller
    {
        private readonly IReportRepository reportRepository;
        private readonly IObstacleRepository obstacleRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public FlightCrewController(
            IReportRepository reportRepository,
            IObstacleRepository obstacleRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.reportRepository = reportRepository;
            this.obstacleRepository = obstacleRepository;
            this.userManager = userManager;
        }

        // GET: FlightCrew/ReportDetails/{id}
        [HttpGet]
        public async Task<IActionResult> ReportDetails(int id)
        {
            ViewData["LayoutType"] = "ipad";
            
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var report = await reportRepository.GetByIdAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            // Verify that the user owns this report
            if (report.AuthorId != user.Id.ToString())
            {
                return Forbid();
            }

            var viewModel = new FlightCrewReportDetailsViewModel
            {
                Id = report.Id,
                Title = report.Title,
                Summary = report.Summary,
                Author = report.Author,
                AuthorId = report.AuthorId,
                ReportDate = report.ReportDate,
                Obstacles = report.Obstacles
            };

            return View(viewModel);
        }

        // GET: FlightCrew/EditReport/{id}
        [HttpGet]
        public async Task<IActionResult> EditReport(int id)
        {
            ViewData["LayoutType"] = "ipad";
            
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var report = await reportRepository.GetByIdAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            // Verify that the user owns this report
            if (report.AuthorId != user.Id.ToString())
            {
                return Forbid();
            }

            var viewModel = new FlightCrewEditReportViewModel
            {
                Id = report.Id,
                Title = report.Title,
                Summary = report.Summary
            };

            return View(viewModel);
        }

        // POST: FlightCrew/EditReport/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReport(int id, FlightCrewEditReportViewModel model)
        {
            ViewData["LayoutType"] = "ipad";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var report = await reportRepository.GetByIdAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            // Verify that the user owns this report
            if (report.AuthorId != user.Id.ToString())
            {
                return Forbid();
            }

            // Update only Title and Summary
            report.Title = model.Title;
            report.Summary = model.Summary;

            await reportRepository.UpdateAsync(report);

            TempData["ReportUpdated"] = true;
            return RedirectToAction("ReportDetails", new { id = report.Id });
        }

        // GET: FlightCrew/ObstacleDetails/{id}
        [HttpGet]
        public async Task<IActionResult> ObstacleDetails(int id)
        {
            ViewData["LayoutType"] = "ipad";
            
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var obstacle = await obstacleRepository.GetObstacleById(id);
            if (obstacle == null)
            {
                return NotFound();
            }

            // Get the report to verify ownership
            var report = await reportRepository.GetByIdAsync(obstacle.ReportId);
            if (report == null)
            {
                return NotFound();
            }

            // Verify that the user owns the report containing this obstacle
            if (report.AuthorId != user.Id.ToString())
            {
                return Forbid();
            }

            return View(obstacle);
        }

        // GET: FlightCrew/EditObstacle/{id}
        [HttpGet]
        public async Task<IActionResult> EditObstacle(int id)
        {
            ViewData["LayoutType"] = "ipad";
            
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var obstacle = await obstacleRepository.GetObstacleById(id);
            if (obstacle == null)
            {
                return NotFound();
            }

            // Get the report to verify ownership
            var report = await reportRepository.GetByIdAsync(obstacle.ReportId);
            if (report == null)
            {
                return NotFound();
            }

            // Verify that the user owns the report containing this obstacle
            if (report.AuthorId != user.Id.ToString())
            {
                return Forbid();
            }

            var viewModel = new FlightCrewEditObstacleRequest
            {
                Id = obstacle.Id,
                ReportId = obstacle.ReportId,
                Type = obstacle.Type,
                Name = obstacle.Name,
                Height = obstacle.Height,
                Latitude = obstacle.Latitude,
                Longitude = obstacle.Longitude,
                Description = obstacle.Description
            };

            return View(viewModel);
        }

        // POST: FlightCrew/EditObstacle/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditObstacle(int id, FlightCrewEditObstacleRequest model)
        {
            ViewData["LayoutType"] = "ipad";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var obstacle = await obstacleRepository.GetObstacleById(id);
            if (obstacle == null)
            {
                return NotFound();
            }

            // Get the report to verify ownership
            var report = await reportRepository.GetByIdAsync(obstacle.ReportId);
            if (report == null)
            {
                return NotFound();
            }

            // Verify that the user owns the report containing this obstacle
            if (report.AuthorId != user.Id.ToString())
            {
                return Forbid();
            }

            // Update obstacle fields (excluding Status)
            obstacle.Type = model.Type;
            obstacle.Name = model.Name;
            obstacle.Height = model.Height;
            obstacle.Latitude = model.Latitude;
            obstacle.Longitude = model.Longitude;
            obstacle.Description = model.Description;
            // Note: Status is NOT updated - FlightCrew cannot change status

            await obstacleRepository.UpdateObstacle(obstacle);

            TempData["ObstacleUpdated"] = true;
            return RedirectToAction("ObstacleDetails", new { id = obstacle.Id });
        }
    }
}

