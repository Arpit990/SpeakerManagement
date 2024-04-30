using Microsoft.AspNetCore.Mvc;

namespace SpeakerManagement.Controllers
{
    public class SpeakerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
