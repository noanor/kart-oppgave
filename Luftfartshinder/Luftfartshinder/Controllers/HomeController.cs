using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Luftfartshinder.Models;
using Luftfartshinder.Models.ViewModel;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;

namespace Luftfartshinder.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDataRepocs _iDataRepository;

        public HomeController(IDataRepocs dataRepocs)
        {
            _iDataRepository = dataRepocs;
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

        [HttpPost]
        public async Task<ActionResult> DataForm(ViewObstacleDataModel serverdata)
        {
            if (serverdata != null && serverdata.ViewObstacleHeight > 0)
            {
                ObstacleData obstacleData = new ObstacleData
                {
                    ObstacleName = serverdata.ViewObstacleName,
                    ObstacleHeight = serverdata.ViewObstacleHeight,
                    ObstacleDescription = serverdata.ViewObstacleDescription,
                    ObstacleCoords = serverdata.ViewObstacleCoords
                };

                var toDatabase = await _iDataRepository.AddObstacle(obstacleData);
                serverdata.ViewObstacleID = toDatabase.ObstacleID;

                return View("Overview", serverdata);

            }


            return BadRequest("Obs, du må fylle inn feltene");
        }
        public IActionResult DataForm(ObstacleData obstacledata)
        {
            return View("Overview", obstacledata);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        
        [Authorize(Roles = "Superadmin")]
        public IActionResult SuperAdminHome()
        {
            return View();
        }

        [Authorize(Roles = "Flybesetning, Superadmin")]
        public IActionResult FlybesetningHome()
        {
            return View();
        }

        [Authorize(Roles = "Registerforer, Superadmin")]
        public IActionResult RegisterforerHome()
        {
            return View();
        }
        
    }
}
