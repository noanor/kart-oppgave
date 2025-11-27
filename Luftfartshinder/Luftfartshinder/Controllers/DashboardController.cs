using Microsoft.AspNetCore.Mvc;
using Luftfartshinder.Models.ViewModel.Shared;
using System.Diagnostics;

namespace Luftfartshinder.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        // Viser en hovedside for "Dashboard"
        public IActionResult Main()
        {
            ViewData["Message"] = "Velkommen til Dashboard!";
            return View();
        }

        // Viser en liste over ting (bare eksempel)
        public IActionResult Items()
        {
            var demoItems = new List<string> { "Kart", "Hinder", "Checkpoint" };
            return View(demoItems);
        }

        // Enkel About-side
        public IActionResult About()
        {
            return View();
        }

        // Feilhåndtering
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
