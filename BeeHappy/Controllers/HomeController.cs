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
        private readonly IUserService _userService;

        public HomeController(
            ILogger<HomeController> logger,
            ITestObjectService testObjectService,
            IUserService userService,
            MongoDBContext mongoDbContext)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            // Test MongoDB connection
            try
            {
                var users = await _userService.GetAllUsersAsync();
                ViewBag.Message += "Current user in DB: " + users.Count;
                ViewBag.Items = users;
            }
            catch (Exception e)
            {
                ViewBag.Message += "MongoDB Error: " + e.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDummyUser(User u)
        {
            try
            {
                // Set default values for required fields
                u.CreatedAt = DateTime.UtcNow;
                u.UpdatedAt = DateTime.UtcNow;
                u.Roles = new List<string>();
                u.Badges = new List<MongoDB.Bson.ObjectId>();
                u.Paints = new List<MongoDB.Bson.ObjectId>();
                u.IsPremium = false;

                await _userService.InsertUserAsync(u);
                TempData["SuccessMessage"] = "User created successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error creating user: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult LandingPage()
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
