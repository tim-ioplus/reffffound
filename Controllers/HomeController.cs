using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using reffffound.Models;

namespace reffffound.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult ChangeLog()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Feed()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Mobile()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Screensaver()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }
        public IActionResult Copyright()
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
