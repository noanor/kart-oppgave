using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Luftfartshinder.Models;
using Luftfartshinder.Models.ViewModel;
using Luftfartshinder.Repository;

namespace Luftfartshinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataRepocs _iDataRepository;

        public HomeController(IDataRepocs dataRepocs)
        {
            _iDataRepository = dataRepocs;
        }

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
                    ObstacleCoords = serverdata.ObstacleCoords
                };

                var toDatabase =  await _iDataRepository.AddObstacle(obstacleData);
                serverdata.ViewObstacleID = toDatabase.ObstacleID;

                return View("Overview", serverdata);

            }


            return BadRequest("Obs, du m� fylle inn feltene");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
