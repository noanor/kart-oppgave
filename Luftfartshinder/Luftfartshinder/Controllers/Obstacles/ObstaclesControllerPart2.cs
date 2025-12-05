using Luftfartshinder.Models.ViewModel.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers.Obstacles
{
    public partial class ObstaclesController : Controller
    {
        // Obstacle liste: PC-vennlig layout (tabell-visning)
        [Authorize(Roles = "Registrar, SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            ViewData["LayoutType"] = "pc";
            var obstacles = await _obstacleRepository.GetAllAsync();
            return View(obstacles);
        }

        // Admin edit obstacle: PC-vennlig layout
        [Authorize(Roles = "Registrar, SuperAdmin")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["LayoutType"] = "pc";
            var existingObstacle = await _obstacleRepository.GetObstacleById(id);

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

        // Delete obstacle
        [Authorize(Roles = "Registrar, SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedObstacle = await _obstacleRepository.DeleteById(id);

            if (deletedObstacle != null)
            {
                TempData["ObstacleDeleted"] = true;
                return RedirectToAction("List");
            }

            return NotFound();
        }
    }
}
