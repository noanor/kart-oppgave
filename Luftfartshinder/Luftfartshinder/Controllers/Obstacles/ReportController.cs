using Luftfartshinder.Extensions;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Obstacles;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers.Obstacles
{
    /// <summary>
    /// Controller for submitting draft obstacles as reports.
    /// </summary>
    public class ReportController : Controller
    {
        private const string DraftKey = "ObstacleDraft";
        private readonly IObstacleRepository _repository;

        /// <summary>
        /// Initializes a new instance of <see cref="ReportController"/>.
        /// </summary>
        /// <param name="repository">Repository for obstacle-related operations.</param>
        public ReportController(IObstacleRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Submits all obstacles from the draft session to the repository.
        /// </summary>
        public async Task<IActionResult> Add()
        {
            var draft = HttpContext.Session.Get<ObstacleDraftViewModel>(DraftKey);
            if (draft is null || draft.Obstacles.Count == 0)
            {
                TempData["Error"] = "No draft to submit.";
                return RedirectToAction("Draft", "Obstacles");
            }

            var errors = new List<string>();
            var successfulObstacles = new List<Obstacle>();

            foreach (var obstacle in draft.Obstacles)
            {
                try
                {
                    await _repository.AddObstacle(obstacle);
                    successfulObstacles.Add(obstacle);
                }
                catch (InvalidOperationException)
                {
                    // Handle known business logic errors - don't expose exception details
                    errors.Add($"Failed to add obstacle '{obstacle.Name}'. Please check your data and try again.");
                }
                catch (Exception)
                {
                    // Log exception here (use ILogger in production)
                    // Return generic error message - never expose exception details
                    errors.Add($"Failed to add obstacle '{obstacle.Name}'. Please try again.");
                }
            }

            // If all obstacles failed, keep the draft and show errors
            if (successfulObstacles.Count == 0)
            {
                TempData["Error"] = "Failed to submit obstacles. " + string.Join(" ", errors);
                return RedirectToAction("Draft", "Obstacles");
            }

            // If some succeeded, remove successful ones from draft
            if (errors.Count > 0)
            {
                // Remove successful obstacles from draft
                foreach (var obstacle in successfulObstacles)
                {
                    draft.Obstacles.Remove(obstacle);
                }
                HttpContext.Session.Set(DraftKey, draft);
                TempData["Warning"] = $"Some obstacles were submitted successfully. Errors: {string.Join(" ", errors)}";
            }
            else
            {
                // All succeeded, clear the draft
                HttpContext.Session.Remove(DraftKey);
                TempData["Success"] = "All obstacles submitted successfully.";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
