using System.Diagnostics;
using BeeHappy.Models;
using BusinessObjects;
using DataAccessObjects;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Services.Implementations;
using Services.Interfaces;

namespace BeeHappy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITestObjectService _testObjectService;
        private readonly MongoDBContext _mongoDbContext;

        public HomeController(ILogger<HomeController> logger, ITestObjectService testObjectService, MongoDBContext mongoDbContext)
        {
            _logger = logger;
            _testObjectService = testObjectService;
            _mongoDbContext = mongoDbContext;
        }

        public async Task<IActionResult> Index()
        {
            // Test MongoDB connection
            try
            {
                var collection = _mongoDbContext.Database.GetCollection<TestObject>("TestObject");
                // var mongoItems = await collection.Find(_ => true).ToListAsync();
                var mongoItems = await _testObjectService.GetAllTestObjects();
                ViewBag.Message += " | MongoDB items found: " + mongoItems.Count;
                ViewBag.Items = mongoItems;
            }
            catch (Exception e)
            {
                ViewBag.Message += " | MongoDB Error: " + e.Message;
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
