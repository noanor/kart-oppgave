using Luftfartshinder.Extensions;
using Luftfartshinder.Models;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

public partial class ObstaclesController : Controller
{
    private const string DraftKey = "ObstacleDraft";
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IReportRepository reportRepository;
    private readonly IObstacleRepository obstacleRepository;

    public record AddOneResponse(bool Ok, int Count, int Index);

    public ObstaclesController(UserManager<ApplicationUser> userManager, IReportRepository reportRepository, IObstacleRepository obstacleRepository)
    {
        this.userManager = userManager;
        this.reportRepository = reportRepository;
        this.obstacleRepository = obstacleRepository;
    }

    // === GET: /obstacles/draft ===
    [HttpGet("/obstacles/draft")]
    public IActionResult Draft()
    {
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

        var index = draft.Obstacles.Count - 1;

        return Ok(new AddOneResponse(true, draft.Obstacles.Count, index));
    }

    // === POST: /obstacles/clear-draft ===
    [HttpPost("/obstacles/clear-draft")]
    public IActionResult ClearDraft()
    {
        HttpContext.Session.Remove(DraftKey);
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
                name = o.Name
            }).ToList();

        return Ok(list);
    }

    // === POST: /obstacles/submit-draft ===
    [Authorize]
    [HttpPost("/obstacles/submit-draft")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitDraft()
    {
        var draft = HttpContext.Session.Get<SessionObstacleDraft>(DraftKey);

        // Create new report
        var newReport = new Report()
        {
            ReportDate = DateTime.Now,
            Author = User.Identity.Name,
            AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Title = ""
        };

        if (draft is null || draft.Obstacles.Count == 0)
        {
            return BadRequest("No draft to submit.");
        }

        // Assign obstacles to the report
        foreach (var obstacle in draft.Obstacles)
        {
            newReport.Obstacles.Add(obstacle);
<<<<<<< HEAD

=======
>>>>>>> 985ea00 (Delete, Approve, Reject, Save Note)
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

    [HttpGet]
    public IActionResult EditObstacle(int index)
    {
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

    // === POST: /obstacles/edit-obstacle ===
    [HttpPost]
    [ValidateAntiForgeryToken]
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    { 
        var existingObstacle = await obstacleRepository.GetObstacleById(id);

        if(existingObstacle == null)
        {
            return NotFound();
        }

        var editObstacleRequest = new EditObstacleRequest
        {
            Id = existingObstacle.Id,
            ReportId = existingObstacle.ReportId,
            Report = existingObstacle.Report,
            Type = existingObstacle.Type,
            Name = existingObstacle.Name,
            Height = existingObstacle.Height,
            Latitude = existingObstacle.Latitude,
            Longitude = existingObstacle.Longitude,
            Description = existingObstacle.Description,
            Status = existingObstacle.Status
        };

        return View("AdminEdit", editObstacleRequest);

        
    }

    public async Task<IActionResult> List()
    {
        var obstacles = await obstacleRepository.GetAllAsync();

        return View(obstacles);
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
