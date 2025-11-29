using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.User;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers.Admin
{
    public class RegistrarController : Controller
    {
        private readonly IReportRepository reportRepository;
        private readonly IObstacleRepository obstacleRepository;
        public RegistrarController(IReportRepository reportRepository, IObstacleRepository obstacleRepository)
        {
            this.reportRepository = reportRepository;
            this.obstacleRepository = obstacleRepository;
        }

        // GET /Registrar
        // Report liste: PC-vennlig layout (tabell-visning)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["LayoutType"] = "pc";
            var reports = await reportRepository.GetAllAsync();
            return View("Index", reports);
        }

        // Report detaljer: PC-vennlig layout
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["LayoutType"] = "pc";
            var report = await reportRepository.GetByIdAsync(id);

            if (report != null)
            {
                var editReport = new EditReportRequest
                {
                    Id = report.Id,
                    Author = report.Author,
                    AuthorId = report.AuthorId,
                    Title = report.Title,
                    Obstacles = report.Obstacles,
                    RegistrarNote = report.RegistrarNote,
                    ReportDate = report.ReportDate
                };

                return View(editReport);

            }
            return NotFound();
        }

        public async Task<IActionResult> SaveNote(Obstacle obstacleData)
        {
            var existingObstacle = await obstacleRepository.GetObstacleById(obstacleData.Id);
            if (existingObstacle != null)
            {
                existingObstacle.RegistrarNote = obstacleData.RegistrarNote;
                await obstacleRepository.UpdateObstacle(existingObstacle);

                TempData["NoteSaved"] = true;
                return RedirectToAction("Details", new { id = existingObstacle.ReportId });
            }

            return RedirectToAction("Details", new { id = obstacleData.ReportId });

        }

        public async Task<IActionResult> Delete(EditReportRequest editReportRequest)
        {
            var deletedReport = await reportRepository.DeleteAsync(editReportRequest.Id);

            if (deletedReport != null)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Details", new { id = editReportRequest.Id });
        }

        public async Task<IActionResult> Approve(int id)
        {
            var obstacle = await obstacleRepository.GetObstacleById(id);
            if (obstacle == null)
                return NotFound(); // prevent NullReference


            obstacle.Status = Obstacle.Statuses.Approved;

            await obstacleRepository.UpdateObstacle(obstacle);

            return RedirectToAction("Details", new { id = obstacle.ReportId });
        }

        public async Task<IActionResult> Reject(int id)
        {
            var obstacle = await obstacleRepository.GetObstacleById(id);

            if (obstacle == null)
                return NotFound(); // prevent NullReference

            if (obstacle != null)
            {
                obstacle.Status = Obstacle.Statuses.Rejected;

                await obstacleRepository.UpdateObstacle(obstacle);

                return RedirectToAction("Details", new { id = obstacle.ReportId });
            }

            return RedirectToAction("Index");
        }

    }
}
