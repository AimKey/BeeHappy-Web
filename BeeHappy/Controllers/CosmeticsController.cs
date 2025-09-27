using System.Security.Claims;
using BusinessObjects;
using CommonObjects.AppConstants;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Services.Interfaces;

namespace BeeHappy.Controllers
{
    public class CosmeticsController(
        IUserService userService,
        IPaintService paintService,
        IPaymentService paymentService,
        ICosmeticsService cosmeticsService) : Controller
    {
        // GET: CosmeticsController
        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                TempData[MessageConstants.MESSAGE] = "Vui lòng đăng nhập";
                TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.WARNING;
                return RedirectToAction("Index", "Home");
            }
            var viewModel = await cosmeticsService.GetUserCosmeticsViewModels(currentUser);
            return View(viewModel);
        }

        // POST: Select Paint
        public async Task<IActionResult> SelectPaint(ObjectId paintId)
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
                await paintService.ActivePaintForUserAsync(currentUser, paintId);
                return Json(new { success = true, message = "Đã chọn màu thành công!" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + e.Message });
            }
        }

        // POST: Deselect Paint (select None)
        public async Task<IActionResult> DeselectPaint()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện chức năng này." });
            }

            try
            {
                await paintService.DeactivateAllPaintsForUserAsync(currentUser);
                return Json(new { success = true, message = "Đã bỏ chọn màu!" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + e.Message });
            }
        }

        private async Task<User?> GetCurrentUserAsync()
        {
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