using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Services.Interfaces;

namespace BeeHappy.Controllers
{
    public class EmoteSetsController(IUserService userService,
                                     IEmoteSetService emoteSetService) : Controller
    {

        // Get all emote sets of the current user
        public async Task<IActionResult> Index()
        {
            var currentUser = GetCurrentUser().Result;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userEmoteSets = await emoteSetService.GetEmoteSetsAsync(es => es.OwnerId == currentUser.Id);
            return View();
        }

        // GET: EmoteSetsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EmoteSetsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmoteSetsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmoteSetsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EmoteSetsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmoteSetsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmoteSetsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private async Task<User?> GetCurrentUser()
        {
            var userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            var user = await userService.GetUserByIdAsync(ObjectId.Parse(userId));
            return user;
        }
    }
}
