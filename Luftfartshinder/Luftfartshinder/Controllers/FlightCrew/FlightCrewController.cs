using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.FlightCrew;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Luftfartshinder.Controllers.FlightCrew
{
    /// <summary>
    /// Controller for FlightCrew users to manage their reports and obstacles.
    /// </summary>
    [Authorize(Roles = "FlightCrew")]
    public class FlightCrewController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IObstacleRepository _obstacleRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of <see cref="FlightCrewController"/>.
        /// </summary>
        /// <param name="reportRepository">Repository for report-related operations.</param>
        /// <param name="obstacleRepository">Repository for obstacle-related operations.</param>
        /// <param name="userManager">User manager for accessing authenticated user data.</param>
        public FlightCrewController(
            IReportRepository reportRepository,
            IObstacleRepository obstacleRepository,
            UserManager<ApplicationUser> userManager)
        {
            _reportRepository = reportRepository;
            _obstacleRepository = obstacleRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Displays details for a specific report.
        /// </summary>
        /// <param name="id">The ID of the report to display.</param>
        [HttpGet]
        public async Task<IActionResult> ReportDetails(int id)
        {
            ViewData["LayoutType"] = "ipad";

            if (id <= 0)
            {
                TempData["Error"] = "Invalid report ID.";
                return RedirectToAction("Index", "Home");
            }
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            try
            {
                var report = await _reportRepository.GetByIdAsync(id);
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
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while loading the report. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Displays the edit form for a specific report.
        /// </summary>
        /// <param name="id">The ID of the report to edit.</param>
        [HttpGet]
        public async Task<IActionResult> EditReport(int id)
        {
            ViewData["LayoutType"] = "ipad";

            if (id <= 0)
            {
                TempData["Error"] = "Invalid report ID.";
                return RedirectToAction("Index", "Home");
            }
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            try
            {
                var report = await _reportRepository.GetByIdAsync(id);
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
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while loading the report. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Updates a report with new data.
        /// </summary>
        /// <param name="id">The ID of the report to update.</param>
        /// <param name="model">The updated report data.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReport(int id, FlightCrewEditReportViewModel model)
        {
            ViewData["LayoutType"] = "ipad";

            if (id <= 0)
            {
                TempData["Error"] = "Invalid report ID.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            try
            {
                var report = await _reportRepository.GetByIdAsync(id);
                if (report == null)
                {
                    TempData["Error"] = "Report not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Verify that the user owns this report
                if (report.AuthorId != user.Id.ToString())
                {
                    TempData["Error"] = "You do not have permission to edit this report.";
                    return RedirectToAction("Index", "Home");
                }

                // Update only Title and Summary
                report.Title = model.Title;
                report.Summary = model.Summary;

                await _reportRepository.UpdateAsync(report);

                TempData["Success"] = "Report updated successfully.";
                return RedirectToAction("ReportDetails", new { id = report.Id });
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError("", "Unable to update the report. Please check your data and try again.");
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while updating the report. Please try again.");
                return View(model);
            }
        }

        /// <summary>
        /// Displays details for a specific obstacle.
        /// </summary>
        /// <param name="id">The ID of the obstacle to display.</param>
        [HttpGet]
        public async Task<IActionResult> ObstacleDetails(int id)
        {
            ViewData["LayoutType"] = "ipad";

            if (id <= 0)
            {
                TempData["Error"] = "Invalid obstacle ID.";
                return RedirectToAction("Index", "Home");
            }
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            try
            {
                var obstacle = await _obstacleRepository.GetObstacleById(id);
                if (obstacle == null)
                {
                    TempData["Error"] = "Obstacle not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Get the report to verify ownership
                var report = await _reportRepository.GetByIdAsync(obstacle.ReportId);
                if (report == null)
                {
                    TempData["Error"] = "Report not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Verify that the user owns the report containing this obstacle
                if (report.AuthorId != user.Id.ToString())
                {
                    TempData["Error"] = "You do not have permission to view this obstacle.";
                    return RedirectToAction("Index", "Home");
                }

                return View(obstacle);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while loading the obstacle. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Displays the edit form for a specific obstacle.
        /// </summary>
        /// <param name="id">The ID of the obstacle to edit.</param>
        [HttpGet]
        public async Task<IActionResult> EditObstacle(int id)
        {
            ViewData["LayoutType"] = "ipad";

            if (id <= 0)
            {
                TempData["Error"] = "Invalid obstacle ID.";
                return RedirectToAction("Index", "Home");
            }
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            try
            {
                var obstacle = await _obstacleRepository.GetObstacleById(id);
                if (obstacle == null)
                {
                    TempData["Error"] = "Obstacle not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Get the report to verify ownership
                var report = await _reportRepository.GetByIdAsync(obstacle.ReportId);
                if (report == null)
                {
                    TempData["Error"] = "Report not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Verify that the user owns the report containing this obstacle
                if (report.AuthorId != user.Id.ToString())
                {
                    TempData["Error"] = "You do not have permission to edit this obstacle.";
                    return RedirectToAction("Index", "Home");
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
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while loading the obstacle. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Updates an obstacle with new data.
        /// </summary>
        /// <param name="id">The ID of the obstacle to update.</param>
        /// <param name="model">The updated obstacle data.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditObstacle(int id, FlightCrewEditObstacleRequest model)
        {
            ViewData["LayoutType"] = "ipad";

            if (id <= 0)
            {
                TempData["Error"] = "Invalid obstacle ID.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            try
            {
                var obstacle = await _obstacleRepository.GetObstacleById(id);
                if (obstacle == null)
                {
                    TempData["Error"] = "Obstacle not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Get the report to verify ownership
                var report = await _reportRepository.GetByIdAsync(obstacle.ReportId);
                if (report == null)
                {
                    TempData["Error"] = "Report not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Verify that the user owns the report containing this obstacle
                if (report.AuthorId != user.Id.ToString())
                {
                    TempData["Error"] = "You do not have permission to edit this obstacle.";
                    return RedirectToAction("Index", "Home");
                }

                // Update obstacle fields (excluding Status)
                obstacle.Type = model.Type?.Trim() ?? string.Empty;
                obstacle.Name = model.Name?.Trim() ?? string.Empty;
                obstacle.Height = model.Height;
                obstacle.Latitude = model.Latitude;
                obstacle.Longitude = model.Longitude;
                obstacle.Description = model.Description?.Trim();
                // Note: Status is NOT updated - FlightCrew cannot change status

                await _obstacleRepository.UpdateObstacle(obstacle);

                TempData["Success"] = "Obstacle updated successfully.";
                return RedirectToAction("ObstacleDetails", new { id = obstacle.Id });
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError("", "Unable to update the obstacle. Please check your data and try again.");
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while updating the obstacle. Please try again.");
                return View(model);
            }
        }
    }
}

