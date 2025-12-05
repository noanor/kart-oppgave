using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.User;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin,Registrar")]
    /// <summary>
    /// Handles pages for Registrar and SuperAdmin to work with reports and obstacles.
    /// </summary>
    public class RegistrarController : Controller
    {
        /// <summary>
        /// Report data storage.
        /// </summary>
        private readonly IReportRepository _reportRepository;

        /// <summary>
        /// Obstacle data storage.
        /// </summary>
        private readonly IObstacleRepository _obstacleRepository;

        /// <summary>
        /// Creates a new controller instance.
        /// </summary>
        /// <param name="reportRepository">Used to read and change reports.</param>
        /// <param name="obstacleRepository">Used to read and change obstacles.</param>
        public RegistrarController(IReportRepository reportRepository, IObstacleRepository obstacleRepository)
        {
            _reportRepository = reportRepository;
            _obstacleRepository = obstacleRepository;
        }
        
        /// <summary>
        /// Shows a table with all reports.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["LayoutType"] = "pc";
            
            try
            {
                var reports = await _reportRepository.GetAllAsync();
                return View("Index", reports);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while loading reports. Please try again.";
                return View("Index", new List<Report>());
            }
        }

        /// <summary>
        /// Shows an overview page with all reports.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            ViewData["LayoutType"] = "pc";
            
            try
            {
                var reports = await _reportRepository.GetAllAsync();
                return View("Overview", reports);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while loading reports. Please try again.";
                return View("Overview", new List<Report>());
            }
        }

        /// <summary>
        /// Shows details for one report.
        /// </summary>
        /// <param name="id">Id of the report.</param>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["LayoutType"] = "pc";
            
            if (id <= 0)
            {
                TempData["Error"] = "Invalid report ID.";
                return RedirectToAction("Overview");
            }

            try
            {
                var report = await _reportRepository.GetByIdAsync(id);

                if (report == null)
                {
                    TempData["Error"] = "Report not found.";
                    return RedirectToAction("Overview");
                }

                return View(MapToEditReport(report));
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while loading the report. Please try again.";
                return RedirectToAction("Overview");
            }
        }

        /// <summary>
        /// Saves the registrar note on an obstacle.
        /// </summary>
        /// <param name="obstacleData">Obstacle with the note text.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveNote(Obstacle obstacleData)
        {
            if (obstacleData == null || obstacleData.Id <= 0)
            {
                TempData["Error"] = "Invalid obstacle data.";
                return RedirectToAction("Overview");
            }

            try
            {
                var existingObstacle = await _obstacleRepository.GetObstacleById(obstacleData.Id);
                if (existingObstacle == null)
                {
                    TempData["Error"] = "Obstacle not found.";
                    return RedirectToAction("Overview");
                }

                // Validate note length
                if (!string.IsNullOrEmpty(obstacleData.RegistrarNote) && obstacleData.RegistrarNote.Length > 1000)
                {
                    TempData["Error"] = "Registrar note cannot exceed 1000 characters.";
                    return RedirectToAction("Details", new { id = existingObstacle.ReportId });
                }

                existingObstacle.RegistrarNote = obstacleData.RegistrarNote;
                await _obstacleRepository.UpdateObstacle(existingObstacle);

                TempData["Success"] = "Note saved successfully.";
                return RedirectToAction("Details", new { id = existingObstacle.ReportId });
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Unable to save the note. Please try again.";
                return RedirectToAction("Details", new { id = obstacleData.ReportId });
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return RedirectToAction("Overview");
            }
        }

        /// <summary>
        /// Deletes a report.
        /// </summary>
        /// <param name="id">Id of the report.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid report ID.";
                return RedirectToAction("Overview");
            }

            try
            {
                var deletedReport = await _reportRepository.DeleteAsync(id);

                if (deletedReport != null)
                {
                    TempData["Success"] = "Report deleted successfully.";
                    return RedirectToAction("Overview");
                }

                TempData["Error"] = "Report not found or could not be deleted.";
                return RedirectToAction("Overview");
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Unable to delete the report. Please try again.";
                return RedirectToAction("Overview");
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return RedirectToAction("Overview");
            }
        }

        /// <summary>
        /// Marks an obstacle as approved.
        /// </summary>
        /// <param name="id">Id of the obstacle.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            return await UpdateObstacleStatus(id, Obstacle.Statuses.Approved);
        }

        /// <summary>
        /// Marks an obstacle as rejected.
        /// </summary>
        /// <param name="id">Id of the obstacle.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            return await UpdateObstacleStatus(id, Obstacle.Statuses.Rejected);
        }

        /// <summary>
        /// Converts a <see cref="Report"/> to an <see cref="EditReportRequest"/>.
        /// </summary>
        /// <param name="report">The report.</param>
        private static EditReportRequest MapToEditReport(Report report)
        {
            return new EditReportRequest
            {
                Id = report.Id,
                Author = report.Author,
                AuthorId = report.AuthorId,
                Title = report.Title,
                Obstacles = report.Obstacles,
                ReportDate = report.ReportDate
            };
        }

        /// <summary>
        /// Changes the status of an obstacle and goes back to the report details page.
        /// </summary>
        /// <param name="id">Id of the obstacle.</param>
        /// <param name="status">New status.</param>
        private async Task<IActionResult> UpdateObstacleStatus(int id, Obstacle.Statuses status)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid obstacle ID.";
                return RedirectToAction("Overview");
            }

            try
            {
                var obstacle = await _obstacleRepository.GetObstacleById(id);
                if (obstacle == null)
                {
                    TempData["Error"] = "Obstacle not found.";
                    return RedirectToAction("Overview");
                }

                obstacle.Status = status;
                await _obstacleRepository.UpdateObstacle(obstacle);

                var statusMessage = status == Obstacle.Statuses.Approved ? "approved" : "rejected";
                TempData["Success"] = $"Obstacle has been {statusMessage}.";
                return RedirectToAction("Details", new { id = obstacle.ReportId });
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Unable to update obstacle status. Please try again.";
                return RedirectToAction("Overview");
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return RedirectToAction("Overview");
            }
        }
    }
}
