using Luftfartshinder.DataContext;
using Luftfartshinder.Extensions;
using Luftfartshinder.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

public partial class ObstaclesController : Controller
{
    private const string DraftKey = "ObstacleDraft";
    private readonly ApplicationContext applicationContext;

    public record AddOneResponse(bool Ok, int Count);

    public ObstaclesController(ApplicationContext applicationContext)
    {
        this.applicationContext = applicationContext;
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
            Longitude = dto.Longitude,
            IsDraft = true
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
        return RedirectToAction("Draft");
    }

    // === POST: /obstacles/submit-draft ===
    [HttpPost("/obstacles/submit-draft")]
    public async Task<IActionResult> SubmitDraft()
    {
        var draft = HttpContext.Session.Get<SessionObstacleDraft>(DraftKey);
        var newReport = new Report()
        {
            ReportDate = DateTime.Now
            
        };

        if (draft is null || draft.Obstacles.Count == 0)
        {
            return BadRequest("No draft to submit.");
        }
        foreach (var obstacle in draft.Obstacles)
        {
            newReport.Obstacles.Add(obstacle);
            applicationContext.Reports.Add(newReport);
        }

        try
        {
            await applicationContext.SaveChangesAsync();
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
    }

    // === POST: /obstacles/edit-obstacle ===
    [HttpPost("/obstacles/edit-obstacle")]
    public IActionResult EditObstacle(int index, [FromBody] Obstacle dto)
    {
        var draft = HttpContext.Session.Get<SessionObstacleDraft>(DraftKey);
        if (draft is null || index < 0 || index >= draft.Obstacles.Count)
        {
            return BadRequest("Invalid draft or index.");
        }
        var obstacle = draft.Obstacles[index];
        obstacle.Type = dto.Type;
        obstacle.Name = dto.Name ?? obstacle.Name;
        obstacle.Description = dto.Description ?? obstacle.Description;
        obstacle.Height = dto.Height;
        obstacle.Latitude = dto.Latitude;
        obstacle.Longitude = dto.Longitude;
        HttpContext.Session.Set(DraftKey, draft);
        return Ok(new { Ok = true });
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
