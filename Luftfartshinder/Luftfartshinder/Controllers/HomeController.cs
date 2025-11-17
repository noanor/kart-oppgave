using Luftfartshinder.Models;
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DataForm()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult SuperAdminHome()
        {
            return View();
        }

        [Authorize(Roles = "Registrar, SuperAdmin")]
        public IActionResult RegistrarHome()
        {
            return View();
        }

        [Authorize(Roles = "FlightCrew, SuperAdmin")]
        public IActionResult IndexHome()
        {
            return View("Index");

        }




        [Authorize]
        public IActionResult Tutorial()
        {
            return View();
        }


    }
}
