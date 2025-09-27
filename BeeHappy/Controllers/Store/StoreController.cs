using System.Security.Claims;
using BusinessObjects;
using CommonObjects.AppConstants;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Services.Interfaces;

namespace BeeHappy.Controllers.Store
{
    public class StoreController(
        IUserService userService,
        IStoreService storeService,
        IPaintService paintService,
        IPaymentService paymentService) : Controller
    {
        // GET: StoreController
        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();
            var viewModel = await storeService.GetStoreIndexVmAsync(currentUser);
            return View(viewModel);
        }

        public async Task<IActionResult> AddPaintToUser(ObjectId paintId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện chức năng này." });
            }

            // Check if user has premium access
            var hasActivePremium = await paymentService.CheckUserHasActivePremium(currentUser);
            if (!hasActivePremium)
            {
                return Json(new { success = false, message = "Bạn cần có gói Premium để sử dụng tính năng này." });
            }

            try
            {
                await paintService.AddPaintToUserAsync(currentUser, paintId);
                return Json(new { success = true, message = "Đã thêm màu vào tài khoản của bạn!" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + e.Message });
            }
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