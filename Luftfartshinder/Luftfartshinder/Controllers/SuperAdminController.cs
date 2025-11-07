using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers
{
    public class SuperAdminController : Controller
    {
        public IActionResult List()
        {
            return View();
        }
    }
}
