using Microsoft.AspNetCore.Mvc;
using Luftfartshinder.Extensions;
using Luftfartshinder.Models.ViewModel;

public partial class ObstaclesController : Controller
{
    private const string DraftKey = "ObstacleDraft";
    [HttpGet("/obstacles/draft")]
    public IActionResult Draft()
    {
        var draft = HttpContext.Session.Get<SessionObstacleDraft>(DraftKey)
                   ?? new SessionObstacleDraft();
        return View("Draft", draft);
    }

    [HttpPost("/obstacles/add-one")]
    public IActionResult AddOne([FromBody] Obstacle dto)
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

        return Ok(new { ok = true, count = draft.Obstacles.Count });
    }
}
