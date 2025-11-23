using Luftfartshinder.Extensions;
using Luftfartshinder.Models.ViewModel.Obstacles;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers
{
    public class ReportController : Controller
    {
        private const string DraftKey = "ObstacleDraft";
        private readonly ObstacleRepository repository;

        public ReportController(ObstacleRepository repository)
        {
            this.repository = repository;
        }
        public async Task<IActionResult> Add()
        {
            var draft = HttpContext.Session.Get<SessionObstacleDraft>(DraftKey);
            if (draft is null || draft.Obstacles.Count == 0)
            {
                return BadRequest("No draft to submit.");
            }
            foreach (var obstacle in draft.Obstacles)
            {
                try
                {
                    await repository.AddObstacle(obstacle);
                }
                catch (Exception ex)
                {
                    // Log the exception (not shown here for brevity)
                    // Most MySQL details are here:
                    Console.WriteLine("DbUpdateException: " + ex.Message);
                    Console.WriteLine("Inner: " + ex.InnerException?.Message);
                    throw; // or return BadRequest with the inner message
                }
            }

            //applicationContext.SaveChanges();
            HttpContext.Session.Remove(DraftKey);
            return RedirectToAction("Index", "Home");
            //return View("Report");
        }
    }
}
