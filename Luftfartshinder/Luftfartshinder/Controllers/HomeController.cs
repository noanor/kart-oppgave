using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Luftfartshinder.Models;
<<<<<<< HEAD
using Luftfartshinder.Models.ViewModel;
using Luftfartshinder.Repository;
=======
>>>>>>> 445730baa88f7cdccb9ead394803ef9045d87d32

namespace Luftfartshinder.Controllers
{
    public class HomeController : Controller
    {
<<<<<<< HEAD
        private readonly IDataRepocs _iDataRepository;

        public HomeController(IDataRepocs dataRepocs)
        {
            _iDataRepository = dataRepocs;
=======
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
>>>>>>> 445730baa88f7cdccb9ead394803ef9045d87d32
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
<<<<<<< HEAD
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


            return BadRequest("Obs, du må fylle inn feltene");
=======
        public IActionResult DataForm(ObstacleData obstacledata)
        {
            return View("Overview", obstacledata);
>>>>>>> 445730baa88f7cdccb9ead394803ef9045d87d32
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
