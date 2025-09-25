using System.Security.Claims;
using BusinessObjects;
using CommonObjects.ViewModels.PaymentVMs;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Services.Interfaces;

namespace BeeHappy.Controllers.Store
{
    public class StoreController(
        IUserService userService,
        IStoreService storeService) : Controller
    {
        // GET: StoreController
        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();
            var viewModel = await storeService.GetStoreIndexVmAsync(currentUser);
            return View(viewModel);
        }
        
        private async Task<User?> GetCurrentUserAsync()
        {
            // var userId = HttpContext.Session.GetString(UserConstants.UserId);
            var userId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            var user = await userService.GetUserByIdAsync(ObjectId.Parse(userId));
            return user;
        }
    }
}
