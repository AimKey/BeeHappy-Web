using System.Diagnostics;
using BeeHappy.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;
using Services.Interfaces;

namespace BeeHappy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITestObjectService _testObjectService;

        public HomeController(ILogger<HomeController> logger, ITestObjectService testObjectService)
        {
            _logger = logger;
            _testObjectService = testObjectService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Message = "DB not configured"
            try
            {
                // var items = await _testObjectService.GetAllTestObjects();
                // ViewBag.Message = "Default items found: " + items.Count + " (2 is good)";
            }
            catch (Exception e)
            {
                ViewBag.Message = "Error: " + e.Message;
            }
            return View();
        }

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
