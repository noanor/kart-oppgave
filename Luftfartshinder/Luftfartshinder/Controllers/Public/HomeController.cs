using Luftfartshinder.Models.ViewModel.Shared;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Luftfartshinder.Controllers
{
    /// <summary>
    /// Main controller that shows the home page and pages based on a user’s role
    /// Handles also choosing the layout and providing common navigation links.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IObstacleRepository _obstacleRepository;

        /// <summary>
        /// Initializes a new instance of <see cref="HomeController"/>.
        /// </summary>
        /// <param name="obstacleRepository">Repository for obstacle-related data.</param>
        public HomeController(IObstacleRepository obstacleRepository)
        {
            _obstacleRepository = obstacleRepository;
        }

        /// <summary>
        /// Sets the layout type used by the view.
        /// </summary>
        /// <param name="layoutType">The layout type ("ipad" or "pc").</param>
        private void SetLayout(string layoutType)
        {
            ViewData["LayoutType"] = layoutType;
        }

        /// <summary>
        /// Main landing page (iPad-friendly map view).
        /// </summary>
        public IActionResult Index()
        {
            SetLayout("ipad");
            ViewData["BodyClass"] = "page-home";
            return View();
        }

        /// <summary>
        /// Privacy information page (desktop layout).
        /// </summary>
        public IActionResult Privacy()
        {
            SetLayout("pc");
            return View();
        }

        /// <summary>
        /// Displays the submission form (iPad-friendly).
        /// </summary>
        [HttpGet]
        public IActionResult DataForm()
        {
            SetLayout("ipad");
            return View();
        }

        /// <summary>
        /// Error page with no caching to ensure correct diagnostic data is always shown.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            SetLayout("pc");
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        /// <summary>
        /// Landing page for SuperAdmin users (desktop layout).
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult SuperAdminHome()
        {
            SetLayout("pc");
            return View();
        }

        /// <summary>
        /// Landing page for Registrar and SuperAdmin roles (desktop layout).
        /// </summary>
        [Authorize(Roles = "Registrar, SuperAdmin")]
        public IActionResult RegistrarHome()
        {
            SetLayout("pc");
            return View();
        }

        /// <summary>
        /// Landing page for FlightCrew. It is also accessible for SuperAdmin users.
        /// This page uses an iPad-friendly layout for easy-to-use tablet devices.
        /// </summary>
        [Authorize(Roles = "FlightCrew, SuperAdmin")]
        public IActionResult IndexHome()
        {
            SetLayout("ipad");
            return View("Index");
        }

        /// <summary>
        /// Tutorial page for authenticated users (iPad-friendly layout).
        /// </summary>
        [Authorize]
        public IActionResult Tutorial()
        {
            SetLayout("ipad");
            return View();
        }
    }
}
