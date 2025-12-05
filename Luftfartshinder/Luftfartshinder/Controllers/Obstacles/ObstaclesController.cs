using Luftfartshinder.Extensions;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Obstacles;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Luftfartshinder.Controllers.Obstacles
{
    /// <summary>
    /// Controller for managing obstacle drafts and submissions.
    /// Handles draft creation, editing, and submission to reports.
    /// </summary>
    public partial class ObstaclesController : Controller
    {
        private const string DraftKey = "ObstacleDraft";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReportRepository _reportRepository;
        private readonly IObstacleRepository _obstacleRepository;

        public record AddOneResponse(bool Ok, int Count);

        /// <summary>
        /// Initializes a new instance of <see cref="ObstaclesController"/>.
        /// </summary>
        /// <param name="userManager">User manager for accessing authenticated user data.</param>
        /// <param name="reportRepository">Repository for report-related operations.</param>
        /// <param name="obstacleRepository">Repository for obstacle-related operations.</param>
        public ObstaclesController(UserManager<ApplicationUser> userManager, IReportRepository reportRepository, IObstacleRepository obstacleRepository)
        {
            _userManager = userManager;
            _reportRepository = reportRepository;
            _obstacleRepository = obstacleRepository;
        }

        /// <summary>
        /// Displays the draft obstacles page (iPad-friendly layout).
        /// </summary>
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

        /// <summary>
        /// Adds a single obstacle to the draft session.
        /// </summary>
        /// <param name="dto">The obstacle data to add.</param>
        /// <returns>JSON response with success status and current draft count.</returns>
        [HttpPost("/obstacles/add-one")]
        public IActionResult AddOne([FromBody] AddObstacleRequest dto)
        {
            if (dto is null)
                return BadRequest(new { error = "No data provided." });

            // Validate the model
            if (!TryValidateModel(dto))
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors.Select(e => new { field = x.Key, message = e.ErrorMessage }))
                    .ToList();
                return BadRequest(new { error = "Validation failed", errors });
            }

            // Additional validation for Type
            if (string.IsNullOrWhiteSpace(dto.Type))
            {
                return BadRequest(new { error = "Obstacle type is required." });
            }

            try
            {
                var draft = HttpContext.Session.Get<ObstacleDraftViewModel>(DraftKey)
                        ?? new ObstacleDraftViewModel();

                var o = new Obstacle
                {
                    Type = dto.Type.Trim(),
                    Name = !string.IsNullOrWhiteSpace(dto.Name) ? dto.Name.Trim() : $"Obstacle {DateTime.UtcNow:HHmmss}",
                    Description = dto.Description?.Trim() ?? "",
                    Height = dto.Height,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude
                };

                // Validate obstacle data matches domain rules
                if (o.Height.HasValue && (o.Height < 0 || o.Height > 200))
                {
                    return BadRequest(new { error = "Height must be between 0 and 200 meters." });
                }

                if (o.Latitude < -90 || o.Latitude > 90)
                {
                    return BadRequest(new { error = "Latitude must be between -90 and 90 degrees." });
                }

                if (o.Longitude < -180 || o.Longitude > 180)
                {
                    return BadRequest(new { error = "Longitude must be between -180 and 180 degrees." });
                }

                draft.Obstacles.Add(o);
                HttpContext.Session.Set(DraftKey, draft);

                return Ok(new AddOneResponse(true, draft.Obstacles.Count));
            }
            catch (InvalidOperationException)
            {
                // Handle known business logic errors
                return BadRequest(new { error = "Unable to add obstacle. Please check your data and try again." });
            }
            catch (Exception)
            {
                // Log exception here (use ILogger in production)
                // Return generic error message - never expose exception details
                return StatusCode(500, new { error = "An error occurred while processing your request. Please try again." });
            }
        }

        /// <summary>
        /// Clears all obstacles from the draft session.
        /// </summary>
        [HttpPost("/obstacles/clear-draft")]
        public IActionResult ClearDraft()
        {
            HttpContext.Session.Remove(DraftKey);
            TempData["DraftCleared"] = true;
            return RedirectToAction("Draft");
        }

        /// <summary>
        /// Submits the draft obstacles as a new report.
        /// </summary>
        /// <param name="model">The report submission data including title and summary.</param>
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
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            if (user.OrganizationId == 0)
            {
                ModelState.AddModelError("", "User is not associated with an organization.");
                return View("Draft", draft);
            }

            if (draft is null || draft.Obstacles.Count == 0)
            {
                ModelState.AddModelError("", "No draft to submit. Please add at least one obstacle.");
                return View("Draft", draft);
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
                await _reportRepository.AddAsync(newReport);
            }
            catch (InvalidOperationException)
            {
                // Handle known business logic errors - show user-friendly message
                ModelState.AddModelError("", "Unable to save your report. Please check your data and try again.");
                return View("Draft", draft);
            }
            catch (Exception)
            {
                // Return user-friendly error message
                ModelState.AddModelError("", "An unexpected error occurred while submitting your report. Please try again later.");
                return View("Draft", draft);
            }

            HttpContext.Session.Remove(DraftKey);
            TempData["DraftSubmitted"] = true;
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Displays the edit form for a draft obstacle (iPad-friendly layout).
        /// </summary>
        /// <param name="index">The index of the obstacle in the draft to edit.</param>
        [HttpGet]
        public IActionResult EditObstacle(int index)
        {
            ViewData["LayoutType"] = "ipad";

            if (index < 0)
            {
                return BadRequest("Invalid obstacle index.");
            }

            var draft = HttpContext.Session.Get<ObstacleDraftViewModel>(DraftKey);
            if (draft is null || index >= draft.Obstacles.Count)
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

        /// <summary>
        /// Updates a draft obstacle with new data.
        /// </summary>
        /// <param name="editObstacleRequest">The updated obstacle data.</param>
        /// <param name="index">The index of the obstacle in the draft to update.</param>
        [HttpPost]
        public IActionResult EditObstacle(EditObstacleRequest editObstacleRequest, int index)
        {
            ViewData["LayoutType"] = "ipad";
            var draft = HttpContext.Session.Get<ObstacleDraftViewModel>(DraftKey);

            if (index < 0)
            {
                return BadRequest("Invalid obstacle index.");
            }

            // Validate model
            if (!ModelState.IsValid)
            {
                if (draft is null || index >= draft.Obstacles.Count)
                {
                    return BadRequest("Invalid draft or index.");
                }
                ViewBag.Index = index;
                return View("EditObstacle", editObstacleRequest);
            }

            if (draft is null || index >= draft.Obstacles.Count)
            {
                return BadRequest("Invalid draft or index.");
            }

            try
            {
                draft.Obstacles[index] = new Obstacle
                {
                    Type = editObstacleRequest.Type?.Trim() ?? string.Empty,
                    Name = editObstacleRequest.Name?.Trim() ?? string.Empty,
                    Height = editObstacleRequest.Height,
                    Latitude = editObstacleRequest.Latitude,
                    Longitude = editObstacleRequest.Longitude,
                    Description = editObstacleRequest.Description?.Trim()
                };

                HttpContext.Session.Set(DraftKey, draft);
                TempData["Success"] = "Obstacle updated successfully.";
                return RedirectToAction("Draft");
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError("", "Unable to update the obstacle. Please check your data and try again.");
                ViewBag.Index = index;
                return View("EditObstacle", editObstacleRequest);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while updating the obstacle. Please try again.");
                ViewBag.Index = index;
                return View("EditObstacle", editObstacleRequest);
            }
        }

        /// <summary>
        /// Returns the current draft obstacles as JSON.
        /// </summary>
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

        /// <summary>
        /// Deletes an obstacle from the draft session.
        /// </summary>
        /// <param name="index">The index of the obstacle to delete.</param>
        [HttpPost]
        public IActionResult DeleteDraftObstacle(int index)
        {
            if (index < 0)
            {
                TempData["Error"] = "Invalid obstacle index.";
                return RedirectToAction("Draft");
            }

            var draft = HttpContext.Session.Get<ObstacleDraftViewModel>(DraftKey);
            if (draft is null || index >= draft.Obstacles.Count)
            {
                TempData["Error"] = "Invalid draft or index.";
                return RedirectToAction("Draft");
            }

            try
            {
                draft.Obstacles.RemoveAt(index);
                HttpContext.Session.Set(DraftKey, draft);
                TempData["Success"] = "Obstacle deleted successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the obstacle. Please try again.";
            }

            return RedirectToAction("Draft");
        }

        /// <summary>
        /// DTO for adding a single obstacle to the draft.
        /// </summary>
        public class AddObstacleRequest
        {
            [Required(ErrorMessage = "Obstacle type is required.")]
            public string? Type { get; set; }

            [Required(ErrorMessage = "Latitude is required.")]
            [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90 degrees.")]
            public double Latitude { get; set; }

            [Required(ErrorMessage = "Longitude is required.")]
            [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180 degrees.")]
            public double Longitude { get; set; }

            [Range(0, 200, ErrorMessage = "Height must be between 0 and 200 meters.")]
            public double? Height { get; set; }

            [StringLength(40, ErrorMessage = "Name cannot exceed 40 characters.")]
            public string? Name { get; set; }

            [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
            public string? Description { get; set; }
        }
    }
}


