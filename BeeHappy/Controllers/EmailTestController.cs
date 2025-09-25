using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace BeeHappy.Controllers
{
    public class EmailTestController : Controller
    {
        private readonly IEmailService _emailService;

        public EmailTestController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        // Test gửi mail thường
        [HttpGet("/email/test")]
        public async Task<IActionResult> TestEmail()
        {
            await _emailService.SendEmailAsync(
                "mjnhtran2004@gmail.com",
                "Test Email",
                "<h2>Đây là email test từ BeeHappy!</h2><p>Nếu bạn thấy mail này tức là service OK ✅</p>"
            );

            return Content("✅ Test email sent successfully!");
        }

        // Test gửi mail Welcome template
        [HttpGet("/email/welcome")]
        public async Task<IActionResult> TestWelcome()
        {
            await _emailService.SendWelcomeEmailAsync(
                "mjnhtran2004@gmail.com",
                "Tran Chi Minh"
            );

            return Content("✅ Welcome email sent successfully!");
        }

        // Test gửi mail Premium template
        [HttpGet("/email/premium")]
        public async Task<IActionResult> TestPremium()
        {
            await _emailService.SendPremiumConfirmationAsync(
                "mjnhtran2004@gmail.com",
                "Tran Chi Minh",
                DateTime.UtcNow.AddMonths(1)
            );

            return Content("✅ Premium confirmation email sent successfully!");
        }
    }
}
