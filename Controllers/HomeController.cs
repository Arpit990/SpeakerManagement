using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakerManagement.Data;
using SpeakerManagement.Models;
using System.Diagnostics;

namespace SpeakerManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() => View();

        [Authorize(Roles = UserRoles.SuperAdmin)]
        public IActionResult SuperAdminDashboard() => View("Dashboard");

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult AdminDashboard() => View("Dashboard");

        [Authorize(Roles = UserRoles.Speaker)]
        public IActionResult SpeakerDashboard() => View("Dashboard");

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
