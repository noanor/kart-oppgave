using Luftfartshinder.Extensions;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Obstacles;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Luftfartshinder.Controllers.Obstacles
{
    public partial class ObstaclesController : Controller
    {
        private const string DraftKey = "ObstacleDraft";
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IReportRepository reportRepository;
        private readonly IObstacleRepository obstacleRepository;

        public record AddOneResponse(bool Ok, int Count);

        public ObstaclesController(UserManager<ApplicationUser> userManager, IReportRepository reportRepository, IObstacleRepository obstacleRepository)
        {
            this.userManager = userManager;
            this.reportRepository = reportRepository;
            this.obstacleRepository = obstacleRepository;
        }

        // === GET: /obstacles/draft ===
        // Draft-visning: iPad-vennlig layout
        [HttpGet("/obstacles/draft")]
        public IActionResult Draft()
        {
            ViewData["LayoutType"] = "ipad";
            // Get existing draft from session, or create a new one
            var draft = HttpContext.Session.Get<ObstacleDraftViewModel>(DraftKey)
                       ?? new ObstacleDraftViewModel();

            // Return the Razor view "Draft.cshtml" with the draft as the model
            return View("Draft", draft);
        }

        // === POST: /obstacles/add-one ===
        [HttpPost("/obstacles/add-one")]
        public IActionResult AddOne([FromBody] AddObstacleRequest dto)
        {
            if (dto is null) return BadRequest("No data");

            var draft = HttpContext.Session.Get<ObstacleDraftViewModel>(DraftKey)
                     ?? new ObstacleDraftViewModel();

            var o = new Obstacle
            {
                Type = dto.Type,
                Name = dto.Name ?? $"Obstacle {DateTime.UtcNow:HHmmss}",
                Description = dto.Description ?? "",
                Height = dto.Height,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude
            };

            draft.Obstacles.Add(o);
            HttpContext.Session.Set(DraftKey, draft);

            return Ok(new AddOneResponse(true, draft.Obstacles.Count));
        }

        // === POST: /obstacles/clear-draft ===
        [HttpPost("/obstacles/clear-draft")]
        public IActionResult ClearDraft()
        {
            HttpContext.Session.Remove(DraftKey);
            TempData["DraftCleared"] = true;
            return RedirectToAction("Draft");
        }

        // === POST: /obstacles/submit-draft ===
        [Authorize]
        [HttpPost("/obstacles/submit-draft")]
        public async Task<IActionResult> SubmitDraft(SubmitDraftViewModel model)
        {
            ViewData["LayoutType"] = "ipad";

            // Get draft from session
            var draft = HttpContext.Session.Get<ObstacleDraftViewModel>(DraftKey)
                       ?? new ObstacleDraftViewModel();

            // Validate model
            if (!ModelState.IsValid)
            {
                // Return to draft view with validation errors
                return View("Draft", draft);
            }

            // 1. Find logged in user
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge(); // or throw
            }

            if (user.OrganizationId == 0)
            {
                return BadRequest("User is not associated with an organization.");
            }

            if (draft is null || draft.Obstacles.Count == 0)
            {
                return BadRequest("No draft to submit.");
            }

            // Create new report
            var newReport = new Report
            {
                ReportDate = DateTime.Now,
                Author = User.Identity.Name,
                AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Title = model.Title,
                Summary = model.Summary,
                OrganizationId = user.OrganizationId
            };

            // Assign obstacles to the report
            foreach (var obstacle in draft.Obstacles)
            {
                obstacle.OrganizationId = user.OrganizationId;
                newReport.Obstacles.Add(obstacle);
            }

            try
            {
                // Send report to DB
                await reportRepository.AddAsync(newReport);
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                // Most MySQL details are here:
                Console.WriteLine("DbUpdateException: " + ex.Message);
                Console.WriteLine("Inner: " + ex.InnerException?.Message);
                throw; // or return BadRequest with the inner message
            }
            
            HttpContext.Session.Remove(DraftKey);
            TempData["DraftSubmitted"] = true;
            return RedirectToAction("Index", "Home");
        }

        // Edit draft obstacle: iPad-vennlig layout
        [HttpGet]
        public IActionResult EditObstacle(int index)
        {
            ViewData["LayoutType"] = "ipad";
            var draft = HttpContext.Session.Get<ObstacleDraftViewModel>(DraftKey);
            if (draft is null || index < 0 || index >= draft.Obstacles.Count)
            {
                return BadRequest("Invalid draft or index.");
            }

            var obstacle = draft.Obstacles[index];

            if (obstacle is not null)
            {
                var editObstacleRequest = new EditObstacleRequest
                {
                    Type = obstacle.Type,
                    Name = obstacle.Name,
                    Height = obstacle.Height,
                    Latitude = obstacle.Latitude,
                    Longitude = obstacle.Longitude,
                    Description = obstacle.Description
                };
                ViewBag.Index = index;
                return View("EditObstacle", editObstacleRequest);
            }

            return NotFound();
        }

        // === POST: /obstacles/edit-obstacle ===
        [HttpPost]
        public IActionResult EditObstacle(EditObstacleRequest editObstacleRequest, int index)
        {
            var draft = HttpContext.Session.Get<ObstacleDraftViewModel>(DraftKey);
            if (draft is null || index < 0 || index >= draft.Obstacles.Count)
            {
                return BadRequest("Invalid draft or index.");
            }

            draft.Obstacles[index] = new Obstacle
            {
                Type = editObstacleRequest.Type,
                Name = editObstacleRequest.Name,
                Height = editObstacleRequest.Height,
                Latitude = editObstacleRequest.Latitude,
                Longitude = editObstacleRequest.Longitude,
                Description = editObstacleRequest.Description
            };



            HttpContext.Session.Set(DraftKey, draft);

            return RedirectToAction("Draft");

        }

        [HttpGet("/obstacles/draft-json")]
        public IActionResult DraftJson()
        {
            var draft = HttpContext.Session
                .Get<ObstacleDraftViewModel>(DraftKey) ?? new ObstacleDraftViewModel();

            var list = draft.Obstacles
                .Select((o, idx) => new
                {
                    index = idx,
                    type = o.Type,
                    latitude = o.Latitude,
                    longitude = o.Longitude,
                    height = o.Height,
                    name = o.Name,
                    description = o.Description
                }).ToList();

            return Ok(list);
        }

        // DTO for JSON requests
        public class AddObstacleRequest
        {
            public string? Type { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double? Height { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
        }
    }
}


