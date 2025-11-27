using Luftfartshinder.Models.ViewModel.Shared;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Luftfartshinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly IObstacleRepository obstacleRepository;

        public HomeController(IObstacleRepository obstacleRepository)
        {
            this.obstacleRepository = obstacleRepository;
        }
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        // Kart-siden: iPad-vennlig layout for touch-interaksjon
        public IActionResult Index()
        {
            ViewData["LayoutType"] = "ipad";
            ViewData["BodyClass"] = "page-home";
            return View();
        }

        // Privacy-side: PC-vennlig layout
        public IActionResult Privacy()
        {
            ViewData["LayoutType"] = "pc";
            return View();
        }

        [HttpGet]
        public IActionResult DataForm()
        {
            ViewData["LayoutType"] = "ipad";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewData["LayoutType"] = "pc";
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult SuperAdminHome()
        {
            ViewData["LayoutType"] = "pc";
            return View();
        }

        [Authorize(Roles = "Registrar, SuperAdmin")]
        public IActionResult RegistrarHome()
        {
            ViewData["LayoutType"] = "pc";
            return View();
        }

        [Authorize(Roles = "FlightCrew, SuperAdmin")]
        public IActionResult IndexHome()
        {
            ViewData["LayoutType"] = "ipad";
            return View("Index");
        }

        // Tutorial: iPad-vennlig layout
        [Authorize]
        public IActionResult Tutorial()
        {
            ViewData["LayoutType"] = "ipad";
            return View();
        }


    }
}
