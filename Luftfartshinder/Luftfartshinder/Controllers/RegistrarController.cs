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
            this.obstacleRepository = obstacleRepository;
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
<<<<<<< HEAD
            return NotFound();
=======
            return null;
>>>>>>> 8bf41d33937aaff5966e809bbf09d35c961b34ea
        }

        public async Task<IActionResult> SaveNote(Obstacle obstacleData)
        {
<<<<<<< HEAD
            var existingObstacle = await obstacleRepository.GetObstacleById(obstacleData.Id);
            if (existingObstacle != null)
            {
                existingObstacle.RegistrarNote = obstacleData.RegistrarNote;
                await obstacleRepository.UpdateObstacle(existingObstacle);

                return RedirectToAction("Details", new { id = existingObstacle.ReportId });
            }

            return RedirectToAction("Details", new { id = obstacleData.ReportId });
=======
            var existingObstacle = obstacleRepository.GetObstacleById(obstacleData.Id).Result;
            if (existingObstacle != null)
            {
                existingObstacle.RegistrarNote = obstacleData.RegistrarNote;
            }

            return RedirectToAction("Details", existingObstacle);
>>>>>>> 8bf41d33937aaff5966e809bbf09d35c961b34ea

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

<<<<<<< HEAD
            if (obstacle == null)
                return NotFound(); // prevent NullReference


            obstacle.Status = Obstacle.Statuses.Approved;

            await obstacleRepository.UpdateObstacle(obstacle);

            return RedirectToAction("Details", new { id = obstacle.ReportId });
=======

            if (obstacle != null)
            {
                obstacle.Status = Obstacle.Statuses.Approved;

                await obstacleRepository.UpdateObstacle(obstacle);

                return RedirectToAction("Details", new { id = obstacle.ReportId });
            }

            return RedirectToAction("Index");
>>>>>>> 8bf41d33937aaff5966e809bbf09d35c961b34ea
        }

        public async Task<IActionResult> Reject(int id)
        {
            var obstacle = await obstacleRepository.GetObstacleById(id);

<<<<<<< HEAD
            if (obstacle == null)
                return NotFound(); // prevent NullReference
=======
>>>>>>> 8bf41d33937aaff5966e809bbf09d35c961b34ea

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

<<<<<<< HEAD

=======
>>>>>>> 8bf41d33937aaff5966e809bbf09d35c961b34ea
