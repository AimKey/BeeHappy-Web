using System.Security.Claims;
using BusinessObjects;
using CommonObjects.AppConstants;
using CommonObjects.ViewModels.PaymentVMs;
using CommonObjects.ViewModels.StoreVMs;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Net.payOS;
using Net.payOS.Types;
using Repositories.Interfaces;
using Services.HelperServices;
using Services.Implementations;
using Services.Interfaces;

namespace BeeHappy.Controllers.Payment;

public class PaymentController(
    IUserService userService,
    IPaymentService paymentService,
    IConfiguration configuration) : Controller
{
    [HttpPost]
    public async Task<IActionResult> SelectPlanAsync(ObjectId planId)
    {
        try
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                throw new Exception("Vui lòng đăng nhập để thực hiện hành động này.");
            }

            // Get the current request's base URL
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var returnUrl = $"{baseUrl}/{configuration["PayOS:SUCCESS_URL_PATH"]}";
            var cancelUrl = $"{baseUrl}/{configuration["PayOS:CANCEL_URL_PATH"]}";
            var createPaymentObj =
                await paymentService.CreatePayOSPaymentObject(currentUser.Id, planId, returnUrl, cancelUrl);
            return Redirect(createPaymentObj.checkoutUrl);
        }
        catch (Exception e)
        {
            TempData[MessageConstants.MESSAGE] =
                "Đã có lỗi xảy ra trong quá trình tạo đơn hàng. Vui lòng thử lại: " + e.Message;
            TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.ERROR;
            return RedirectToAction("Index", "Store");
        }
    }

    [HttpGet]
    public IActionResult Success()
    {
        var responseObj = PaymentUtils.ParseResponse(Request.Query);
        var viewModel = new PaymentResultVM
        {
            IsSuccess = true,
            Title = "Thanh toán thành công!",
            Message = "Cảm ơn bạn đã nâng cấp gói dịch vụ BeeHappy. Tài khoản của bạn đã được kích hoạt.",
            OrderCode = responseObj.OrderCode.ToString(),
        };
        // Update user's purchase history
        paymentService.CompletePurchaseHistoryForUser(responseObj.OrderCode.GetValueOrDefault());
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Cancel()
    {
        var responseObj = PaymentUtils.ParseResponse(Request.Query);
        var viewModel = new PaymentResultVM
        {
            IsSuccess = false,
            Title = "Thanh toán thất bại",
            Message = "Đã có lỗi xảy ra trong quá trình thanh toán. Vui lòng thử lại sau.",
            OrderCode = responseObj.OrderCode.ToString(),
        };
        paymentService.CancelPurchaseHistoryForUser(responseObj.OrderCode.GetValueOrDefault());
        return View("Failed", viewModel);
    }

    [HttpGet]
    public IActionResult Failed()
    {
        var responseObj = PaymentUtils.ParseResponse(Request.Query);
        var viewModel = new PaymentResultVM
        {
            IsSuccess = false,
            Title = "Thanh toán không thành công",
            Message = "Giao dịch của bạn đã bị hủy hoặc không thể hoàn tất.",
            OrderCode = responseObj.OrderCode.ToString(),
        };
        paymentService.CancelPurchaseHistoryForUser(responseObj.OrderCode.GetValueOrDefault());
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