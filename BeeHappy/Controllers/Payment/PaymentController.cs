using BusinessObjects;
using CommonObjects.AppConstants;
using CommonObjects.ViewModels.PaymentVMs;
using CommonObjects.ViewModels.StoreVMs;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Net.payOS;
using Net.payOS.Types;
using PostHog;
using Repositories.Interfaces;
using Services.CronjobServices;
using Services.HelperServices;
using Services.Implementations;
using Services.Interfaces;
using System.Security.Claims;

namespace BeeHappy.Controllers.Payment;

public class PaymentController(
    IUserService userService,
    IPaymentService paymentService,
    IConfiguration configuration,
    IPostHogClient posthog,
      IEmailService emailService) : Controller
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
            // Track event vào PostHog
            posthog.Capture(
                User.Identity?.Name ?? "guest",
                eventName: "Select Plan Clicked",
                properties: new Dictionary<string, object>
                {
                { "planId", planId }
                }
            );

            // Get the current request's base URL
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var returnUrl = $"{baseUrl}/{configuration["PayOS:SUCCESS_URL_PATH"]}";
            var cancelUrl = $"{baseUrl}/{configuration["PayOS:CANCEL_URL_PATH"]}";
            var createPaymentObj =
                await paymentService.CreatePayOSPaymentObject(currentUser.Id, planId, returnUrl, cancelUrl);

            // Add cronjob here
            BackgroundJob.Schedule<PaymentJob>(
                job => job.CancelIfUnpaidAsync(createPaymentObj.orderCode),
                TimeSpan.FromMinutes(PaymentConstants.PAYMENT_CANCEL_AFTER));

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
    public async Task<IActionResult> Success()
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

        var currentUser = await GetCurrentUserAsync();

        DateTime? expireAt = null;
        if (currentUser != null)
        {
            var planInfo = await userService.GetCurrentPlanAsync(currentUser.Id);
            expireAt = planInfo?.ExpiryDate ?? DateTime.UtcNow.AddDays(30); // fallback
        }

        // ✅ Gửi email xác nhận premium
        if (currentUser != null && expireAt != null)
        {
            await emailService.SendPremiumConfirmationAsync(
                currentUser.Email,
                currentUser.Username ?? "Người dùng",
                expireAt.Value
            );
        }

        // Track vào PostHog
        posthog.Capture(
            User.Identity?.Name ?? "guest",
            eventName: "Payment Success",
            properties: new Dictionary<string, object>
            {
            { "orderCode", responseObj.OrderCode },
            { "transactionId", responseObj.Id },
            { "status", "success" },
            { "userId", User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value }
            }
        );
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
        // Track vào PostHog
        posthog.Capture(
            User.Identity?.Name ?? "guest",
            eventName: "Payment Cancelled",
            properties: new Dictionary<string, object>
            {
            { "orderCode", responseObj.OrderCode },
            { "transactionId", responseObj.Id },
            { "status", "cancelled" },
            { "userId", User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value }
            }
        );
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
        // Track vào PostHog        
        posthog.Capture(
            User.Identity?.Name ?? "guest",
            eventName: "Payment Failed",
            properties: new Dictionary<string, object>
            {
            { "orderCode", responseObj.OrderCode },
            { "transactionId", responseObj.Id },
            { "status", "failed" },
            { "userId", User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value }
            }
        );
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