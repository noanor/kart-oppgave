//using Microsoft.AspNetCore.Mvc;
//using Luftfartshinder.Repository;
//using Luftfartshinder.Models.Domain;

//namespace Luftfartshinder.Controllers
//{
//    public class MapController : Controller
//    {
//        private readonly IDataRepocs _IdataRepocs;

//        public MapController(IDataRepocs iDatarepoc) 
//        {
//            _IdataRepocs = iDatarepoc;
//        }

//        [HttpGet] //Get er for å vise frontenden
//        public async Task <ActionResult> Report()
//        {
//            return View();
//        }
//        [HttpPost]
//        public async Task<ActionResult> Report(Obstacle request)
//        {
//            return View();
//        }
//    }

//}
