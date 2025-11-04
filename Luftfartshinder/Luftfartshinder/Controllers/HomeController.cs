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
        public async Task<ActionResult> DataForm(Obstacle serverdata)
        {
            if (serverdata != null && serverdata.Height > 0)
            {
                Obstacle obstacleData = new Obstacle
                {
                    Name = serverdata.Name,
                    Height = serverdata.Height,
                    Description = serverdata.Description,
                    Latitude = serverdata.Latitude,

                    Longitude = serverdata.Longitude
                };

                var toDatabase = await _iDataRepository.AddObstacle(obstacleData);
                serverdata.Id = toDatabase.Id;

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
