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
            var reports = await _reportRepository.GetAllAsync();
            return View("Index", reports);
        }

        /// <summary>
        /// Shows an overview page with all reports.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            ViewData["LayoutType"] = "pc";
            var reports = await _reportRepository.GetAllAsync();
            return View("Overview", reports);
        }

        /// <summary>
        /// Shows details for one report.
        /// </summary>
        /// <param name="id">Id of the report.</param>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["LayoutType"] = "pc";
            var report = await _reportRepository.GetByIdAsync(id);

            if (report == null)
            {
                return NotFound();
            }

            return View(MapToEditReport(report));
        }

        /// <summary>
        /// Saves the registrar note on an obstacle.
        /// </summary>
        /// <param name="obstacleData">Obstacle with the note text.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveNote(Obstacle obstacleData)
        {
            var existingObstacle = await _obstacleRepository.GetObstacleById(obstacleData.Id);
            if (existingObstacle != null)
            {
                existingObstacle.RegistrarNote = obstacleData.RegistrarNote;
                await _obstacleRepository.UpdateObstacle(existingObstacle);

                TempData["NoteSaved"] = true;
                return RedirectToAction("Details", new { id = existingObstacle.ReportId });
            }

            return RedirectToAction("Details", new { id = obstacleData.ReportId });

        }

        /// <summary>
        /// Deletes a report.
        /// </summary>
        /// <param name="id">Id of the report.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedReport = await _reportRepository.DeleteAsync(id);

            if (deletedReport != null)
            {
                return RedirectToAction("Overview");
            }

            return RedirectToAction("Details", new { id = id });
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
            var obstacle = await _obstacleRepository.GetObstacleById(id);
            if (obstacle == null)
            {
                return NotFound();
            }

            obstacle.Status = status;
            await _obstacleRepository.UpdateObstacle(obstacle);

            return RedirectToAction("Details", new { id = obstacle.ReportId });
        }
    }
}
