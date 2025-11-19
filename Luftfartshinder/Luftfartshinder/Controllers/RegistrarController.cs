using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers
{
    public class RegistrarController : Controller
    {
        private readonly IReportRepository reportRepository;
        private readonly IObstacleRepository obstacleRepository;
        public RegistrarController(IReportRepository reportRepository, IObstacleRepository obstacleRepository)
        {
            this.reportRepository = reportRepository;
        }

        // GET /Registrar
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reports = await reportRepository.GetAllAsync();
            return View("Index", reports);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var report = reportRepository.GetByIdAsync(id).Result;

            if (report != null)
            {
                var editReport = new EditReportRequest
                {
                    Id = report.Id,
                    Author = report.Author,
                    AuthorId = report.AuthorId,
                    Obstacles = report.Obstacles,
                    RegistrarNote = report.RegistrarNote,
                    ReportDate = report.ReportDate
                };

                return View(editReport);

            }
            return null;
        }

        public async Task<IActionResult> SaveNote(Obstacle obstacleData)
        {
            var existingObstacle = obstacleRepository.GetObstacleById(obstacleData.Id).Result;
            if (existingObstacle != null)
            {
                existingObstacle.RegistrarNote = obstacleData.RegistrarNote;
            }

            return RedirectToAction("Details", existingObstacle);

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


            if (obstacle != null)
            {
                obstacle.Status = Obstacle.Statuses.Approved;

                await obstacleRepository.UpdateObstacle(obstacle);

                return RedirectToAction("Details", new { id = obstacle.ReportId });
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Reject(int id)
        {
            var obstacle = await obstacleRepository.GetObstacleById(id);


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


