using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers
{
    public class AdviceController : Controller
    {
        public IActionResult Index()
        {
            string advice = "Husk å drikke vann mens du jobber";
            return View("Index", advice);
        }

    }
}
