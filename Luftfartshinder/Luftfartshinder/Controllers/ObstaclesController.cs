using Luftfartshinder.Extensions;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Admin;
using Luftfartshinder.Models.ViewModel.Obstacles;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        var draft = HttpContext.Session.Get<SessionObstacleDraft>(DraftKey)
                   ?? new SessionObstacleDraft();

        // Return the Razor view "Draft.cshtml" with the draft as the model
        return View("Draft", draft);
    }

    // === POST: /obstacles/add-one ===
    [HttpPost("/obstacles/add-one")]
    public IActionResult AddOne([FromBody] AddObstacleRequest dto)
    {
        if (dto is null) return BadRequest("No data");

        var draft = HttpContext.Session.Get<SessionObstacleDraft>(DraftKey)
                 ?? new SessionObstacleDraft();

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
    public async Task<IActionResult> SubmitDraft()
    {
        var draft = HttpContext.Session.Get<SessionObstacleDraft>(DraftKey);

        // 1. Finn innlogget bruker
        var user = await userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge(); // eller throw
        }

        if (user.OrganizationId == null)
        {
            return BadRequest("Brukeren er ikke knyttet til en organisasjon.");
        }

        // Create new report
        var newReport = new Report()
        {
            ReportDate = DateTime.Now,
            Author = User.Identity.Name,
            AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Title = "",
            OrganizationId = user.OrganizationId
        };

        if (draft is null || draft.Obstacles.Count == 0)
        {
            return BadRequest("No draft to submit.");
        }

        // Assign obstacles to the report
        for (int i = 0; i < draft.Obstacles.Count; i++)
        {
            var obstacle = draft.Obstacles[i];
            obstacle.OrganizationId = user.OrganizationId;
            
            // Save line points if they exist for this obstacle
            if (draft.LinePoints != null && draft.LinePoints.ContainsKey(i) && draft.LinePoints[i] != null && draft.LinePoints[i].Count > 1)
            {
                try
                {
                    // Convert line points to JSON string
                    var linePointsJson = System.Text.Json.JsonSerializer.Serialize(draft.LinePoints[i]);
                    obstacle.LinePointsJson = linePointsJson;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error serializing line points for obstacle {i}: {ex.Message}");
                    // Continue without line points if serialization fails
                }
            }
            
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
        //applicationContext.SaveChanges();
        HttpContext.Session.Remove(DraftKey);
        TempData["DraftSubmitted"] = true;
        return RedirectToAction("Index", "Home");
    }

    // Edit draft obstacle: iPad-vennlig layout
    [HttpGet]
    public IActionResult EditObstacle(int index)
    {
        ViewData["LayoutType"] = "ipad";
        ViewData["ObstacleIndex"] = index;
        var draft = HttpContext.Session.Get<SessionObstacleDraft>(DraftKey);
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
            return View("EditObstacle", editObstacleRequest);
        }

        return View(null);
    }

    // Obstacle liste: PC-vennlig layout (tabell-visning)
    [HttpGet]
    public async Task<IActionResult> List()
    {
        ViewData["LayoutType"] = "pc";
        var obstacles = await obstacleRepository.GetAllAsync();
        return View(obstacles);
    }

    // === POST: /obstacles/edit-obstacle ===
    [HttpPost]
    public IActionResult EditObstacle(EditObstacleRequest editObstacleRequest, int index)
    {
        var draft = HttpContext.Session.Get<SessionObstacleDraft>(DraftKey);
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
            .Get<SessionObstacleDraft>(DraftKey) ?? new SessionObstacleDraft();

        var list = draft.Obstacles
            .Select((o, idx) => new
            {
                index = idx,
                type = o.Type,
                latitude = o.Latitude,
                longitude = o.Longitude,
                name = o.Name,
                linePoints = draft.LinePoints.ContainsKey(idx) ? draft.LinePoints[idx] : null
            }).ToList();

        return Ok(list);
    }

    // === POST: /obstacles/save-line-points ===
    [HttpPost("/obstacles/save-line-points")]
    public IActionResult SaveLinePoints([FromBody] SaveLinePointsRequest request)
    {
        if (request == null || request.Index < 0)
        {
            return BadRequest("Invalid request");
        }

        var draft = HttpContext.Session.Get<SessionObstacleDraft>(DraftKey)
                 ?? new SessionObstacleDraft();

        if (request.Index >= draft.Obstacles.Count)
        {
            return BadRequest("Invalid obstacle index");
        }

        // Store line points for this obstacle index
        if (draft.LinePoints == null)
        {
            draft.LinePoints = new Dictionary<int, List<double[]>>();
        }

        if (request.Points != null && request.Points.Count > 0)
        {
            draft.LinePoints[request.Index] = request.Points;
        }
        else
        {
            // Remove line points if empty
            draft.LinePoints.Remove(request.Index);
        }
        
        HttpContext.Session.Set(DraftKey, draft);

        return Ok(new { success = true, pointCount = request.Points?.Count ?? 0 });
    }

    public class SaveLinePointsRequest
    {
        public int Index { get; set; }
        public List<double[]>? Points { get; set; }
    }

    // Admin edit obstacle: PC-vennlig layout
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["LayoutType"] = "pc";
        var existingObstacle = await obstacleRepository.GetObstacleById(id);

        if (existingObstacle == null)
        {
            return NotFound();
        }

        var editObstacleRequest = new AdminEditObstacleRequest()
        {
            Id = existingObstacle.Id,
            Type = existingObstacle.Type,
            Height = existingObstacle.Height,
            Description = existingObstacle.Description,
            Latitude = existingObstacle.Latitude,
            Longitude = existingObstacle.Longitude,
            Report = existingObstacle.Report,
            ReportId = existingObstacle.ReportId,
            Name = existingObstacle.Name,
            RegistrarNote = existingObstacle.RegistrarNote,
            Status = existingObstacle.Status
        };

        return View("AdminEdit", editObstacleRequest);
    }

    // Add Image view: Visual only, not functional
    [HttpGet]
    public IActionResult AddImage(int? index, int? id)
    {
        if (index.HasValue)
        {
            ViewData["LayoutType"] = "ipad";
            ViewData["ObstacleIndex"] = index.Value;
            ViewData["IsDraft"] = true;
        }
        else if (id.HasValue)
        {
            ViewData["LayoutType"] = "pc";
            ViewData["ObstacleId"] = id.Value;
            ViewData["IsDraft"] = false;
        }
        else
        {
            return BadRequest("Either index or id must be provided.");
        }

        return View();
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
