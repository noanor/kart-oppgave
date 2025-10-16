using Microsoft.AspNetCore.Mvc;
using Oppgave1.Models.ViewModel;
using Oppgave1.Repository;

namespace Oppgave1.Controllers
{
    public class MapController : Controller
    {
        private readonly IDataRepocs _IdataRepocs;

        public MapController(IDataRepocs iDatarepoc) 
        {
            _IdataRepocs = iDatarepoc;
        }

        [HttpGet] //Get er for å vise frontenden
        public async Task <ActionResult> Report()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Report(ViewObstacleDataModel request)
        {
            return View();
        }
    }

}
