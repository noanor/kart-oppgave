using Luftfartshinder.DataContext;
using Luftfartshinder.Extensions;
using Luftfartshinder.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers
{
    public class ReportController : Controller
    {
        private const string DraftKey = "ObstacleDraft";
        private readonly ApplicationContext applicationContext;

        public ReportController(ApplicationContext applicationContext)
        {
            this.applicationContext = applicationContext;
        }
        public IActionResult Add()
        {
            var draft = HttpContext.Session.Get<SessionObstacleDraft>(DraftKey);
            if (draft is null || draft.Obstacles.Count == 0)
            {
                return BadRequest("No draft to submit.");
            }
            foreach (var obstacle in draft.Obstacles)
            {
                //obstacle.IsDraft = false;
                applicationContext.Obstacles.Add(obstacle);
            }

            try
            {
                applicationContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                // Most MySQL details are here:
                Console.WriteLine("DbUpdateException: " + ex.Message);
                Console.WriteLine("Inner: " + ex.InnerException?.Message);
                throw; // or return BadRequest with the inner message
            }
            //applicationContext.SaveChanges();
            HttpContext.Session.Remove(DraftKey);
            return RedirectToAction("Index", "Home");
            //return View("Report");
        }
    }
}
